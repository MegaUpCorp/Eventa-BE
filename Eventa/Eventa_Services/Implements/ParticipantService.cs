using Eventa_BusinessObject.DTOs.Account;
using Eventa_BusinessObject.Entities;
using Eventa_Repositories.Implements;
using Eventa_Repositories.Interfaces;
using QRCoder;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text;
using System.Threading.Tasks;
using Eventa_Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Appwrite;
using Appwrite.Models;
using System.Net.Http;
using System.Net.Http.Headers;

namespace Eventa_Services.Implement
{
    public class ParticipantService : IParticipantService
    {
        private readonly IParticipantRepository _participantRepository;
        private readonly IEventRepository _eventRepository;
        private readonly IEmailService _emailService;
        private readonly IAccountRepository _accountRepository;
        private readonly Client _appwriteClient; // Appwrite Client
        private readonly string _bucketId; // Bucket ID cho QR code
        private readonly string _projectId; // Project ID
        private readonly string _apiKey; // API Key

        public ParticipantService(
            IParticipantRepository participantRepository,
            IEventRepository eventRepository,
            IEmailService emailService,
            IAccountRepository accountRepository)
        {
            _participantRepository = participantRepository;
            _eventRepository = eventRepository;
            _emailService = emailService;
            _accountRepository = accountRepository;

            // Khởi tạo Appwrite Client với thông tin bạn cung cấp
            _appwriteClient = new Client();
            _appwriteClient
                .SetEndpoint("https://cloud.appwrite.io/v1") // Appwrite Endpoint
                .SetProject("67bc78cb002225a750d4") // Project ID
                .SetKey("standard_1a8297ce2832abd0fb5f88c473250c64d432cae1ac923cb988908d0c7d6e3182c34f49c40b75b1da3af88239a6d875cdcf1f595e05f331286b73b7d15f715924f17b7cf38e705f3d713b35f1969df576a348aa2c298c0727e8b7636e57de9daf6e5bd90a8c74a78c6f0df05b36eb92831960f400b1d5fdfc7b5c201ba7e3585c"); // API Key

            _bucketId = "67bc7909001dae69bd46"; // Bucket ID cho hình ảnh
            _projectId = "67bc78cb002225a750d4"; // Project ID
            _apiKey = "standard_1a8297ce2832abd0fb5f88c473250c64d432cae1ac923cb988908d0c7d6e3182c34f49c40b75b1da3af88239a6d875cdcf1f595e05f331286b73b7d15f715924f17b7cf38e705f3d713b35f1969df576a348aa2c298c0727e8b7636e57de9daf6e5bd90a8c74a78c6f0df05b36eb92831960f400b1d5fdfc7b5c201ba7e3585c"; // API Key

        }

        public async Task<List<Participant>> GetParticipantsByEventId(Guid eventId)
        {
            return await _participantRepository.GetByEventIdAsync(eventId);
        }

        public async Task<bool> RegisterParticipant(Guid accountId, Guid eventId)
        {
            if (accountId == Guid.Empty || eventId == Guid.Empty)
                throw new InvalidOperationException("Account ID hoặc Event ID không hợp lệ.");

            var eventItem = await _eventRepository.GetById(eventId);
            if (eventItem == null)
                throw new InvalidOperationException("Sự kiện không tồn tại.");

            var currentParticipants = await _participantRepository.GetByEventIdAsync(eventId);
            if (currentParticipants.Count >= eventItem.Capacity)
                throw new InvalidOperationException("Sự kiện đã đầy, không thể đăng ký thêm.");

            var existingParticipant = await _participantRepository.GetAsync(p => p.AccountId == accountId && p.EventId == eventId);
            if (existingParticipant != null)
                throw new InvalidOperationException("Bạn đã đăng ký cho sự kiện này rồi.");

            var participant = new Participant
            {
                Id = Guid.NewGuid(),
                AccountId = accountId,
                EventId = eventId,
                IsConfirmed = eventItem.RequiresApproval ? false : true,
                IsCheckedIn = false
            };

            var result = await _participantRepository.AddAsync(participant);
            if (!result)
                throw new InvalidOperationException("Không thể thêm participant vào cơ sở dữ liệu.");

            var account = await _accountRepository.GetAsync(accountId);
            if (account == null || string.IsNullOrEmpty(account.Email))
                throw new InvalidOperationException("Không tìm thấy email của tài khoản.");

            // Tạo QR code và upload lên Appwrite
            string imgURl = string.Empty;
            try
            {
                // Tạo QR code
                Console.WriteLine("Bắt đầu tạo QR code...");
                var qrData = $"AccountId={accountId}&ParticipantId={participant.Id}&EventId={eventId}";
                var qrGenerator = new QRCodeGenerator();
                var qrCodeData = qrGenerator.CreateQrCode(qrData, QRCodeGenerator.ECCLevel.Q);
                var qrCode = new QRCode(qrCodeData);
                using var qrBitmap = qrCode.GetGraphic(20);

                // Lưu QR code thành file tạm
                var qrFilePath = Path.Combine(Path.GetTempPath(), $"qrcode_{participant.Id}.png");
                Console.WriteLine($"Lưu QR code vào: {qrFilePath}");
                qrBitmap.Save(qrFilePath, ImageFormat.Png);

                // Upload file lên Appwrite Storage
                Console.WriteLine("Bắt đầu upload QR code lên Appwrite...");
                if (_appwriteClient == null)
                {
                    throw new InvalidOperationException("Appwrite client is not initialized.");
                }

                var storage = new Appwrite.Services.Storage(_appwriteClient);
                if (storage == null)
                {
                    throw new InvalidOperationException("Storage service is not initialized.");
                }

                using var fileStream = new FileStream(qrFilePath, FileMode.Open, FileAccess.Read);
                if (fileStream == null)
                {
                    throw new InvalidOperationException("Không thể mở file QR code.");
                }

                var inputFile = InputFile.FromStream(fileStream, $"qrcode_{participant.Id}.png", "image/png");
                if (inputFile == null)
                {
                    throw new InvalidOperationException("inputFile is null. Không thể đọc file QR code.");
                }

                Console.WriteLine($"Appwrite Client: {_appwriteClient != null}");
                Console.WriteLine($"Storage Service: {storage != null}");
                Console.WriteLine($"Bucket ID: {_bucketId}");
                Console.WriteLine($"File Path: {qrFilePath}");
                Console.WriteLine($"inputFile: {inputFile != null}");

                Appwrite.Models.File appwriteFile;
                try
                {
                    appwriteFile = await storage.CreateFile(
                        bucketId: _bucketId,
                        fileId: ID.Unique(),
                        file: inputFile
                    );
                    Console.WriteLine($"Upload thành công. File ID: {appwriteFile.Id}");
                }
                catch (AppwriteException ex)
                {
                    Console.WriteLine($"Lỗi từ Appwrite: Code={ex.Code}, Message={ex.Message}, Response={ex.Response}");
                    throw new InvalidOperationException($"Lỗi khi upload file lên Appwrite: {ex.Message}", ex);
                }

                if (appwriteFile != null)
                {
                    imgURl = $"https://cloud.appwrite.io/v1/storage/buckets/{_bucketId}/files/{appwriteFile.Id}/view?project=67bc78cb002225a750d4";
                    Console.WriteLine($"QR Code URL: {imgURl}");
                }
                else
                {
                    throw new InvalidOperationException("Upload file lên Appwrite thất bại: appwriteFile là null.");
                }

                // Xóa file tạm
                if (System.IO.File.Exists(qrFilePath))
                {
                    Console.WriteLine("Xóa file tạm...");
                    System.IO.File.Delete(qrFilePath);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi tạo hoặc upload QR code: {ex.Message}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");
                throw new InvalidOperationException("Không thể tạo hoặc upload QR code.", ex);
            }

            // Tạo email body với link QR code
            var emailBody = $@"
                <h2>Đăng ký sự kiện thành công</h2>
                <p>Cảm ơn bạn đã đăng ký tham gia sự kiện <strong>{eventItem.Title}</strong>.</p>
                <p>Vui lòng sử dụng QR code dưới đây để check-in tại sự kiện:</p>
                <img src='{imgURl}' alt='QR Code' style='width:200px;height:200px;' />
                <p>Nếu không thấy hình ảnh, bạn có thể tải QR code tại: <a href='{imgURl}'>Tải QR Code</a></p>
                <p>Thông tin:<br/>
                   - Account ID: {accountId}<br/>
                   - Participant ID: {participant.Id}<br/>
                   - Event ID: {eventId}</p>";

            // Gửi email
            var emailSent = await _emailService.SendEmailAsync(account.Email, "Xác nhận đăng ký sự kiện", emailBody);
            if (!emailSent)
                Console.WriteLine("Gửi email thất bại.");

            return true;
        }

        public async Task<bool> RemoveParticipant(Guid participantId)
        {
            var participant = await _participantRepository.GetAsync(participantId);
            if (participant == null)
                throw new InvalidOperationException("Participant không tồn tại.");

            var result = await _participantRepository.DeleteAsync(participant);
            if (!result)
                throw new InvalidOperationException("Không thể xóa participant khỏi cơ sở dữ liệu.");

            return true;
        }
        public async Task<List<AccountDTO1>> GetParticipantsOfEvent(string slug)
        {
            var participants = await _participantRepository.GetParticipantsOfEvent(slug);
            var accountIds = participants.Select(p => p.AccountId).Distinct().ToList();
            var accounts = await _accountRepository.GetAllAsync(a => accountIds.Contains(a.Id));
            return accounts.Select(a => new AccountDTO1
            {
                Id = a.Id,
                Email = a.Email,
               // Username = a.Username,
                FullName = a.FullName,
                ProfilePicture = a.ProfilePicture
            }).ToList();
        }
    }
}