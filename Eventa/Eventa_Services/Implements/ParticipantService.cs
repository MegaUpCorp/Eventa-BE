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
using Microsoft.Extensions.Logging;
using Eventa_Services.Implements;

namespace Eventa_Services.Implement
{
    public class ParticipantService : IParticipantService
    {
        private readonly IParticipantRepository _participantRepository;
        private readonly IEventRepository _eventRepository;
        private readonly IEmailService _emailService;
        private readonly IAccountRepository _accountRepository;
        private readonly IFirebaseService _firebaseService;
        private readonly ILogger<ParticipantService> _logger;

        public ParticipantService(
            IParticipantRepository participantRepository,
            IEventRepository eventRepository,
            IEmailService emailService,
            IAccountRepository accountRepository,
            IFirebaseService firebaseService,
            ILogger<ParticipantService> logger)
        {
            _participantRepository = participantRepository;
            _eventRepository = eventRepository;
            _emailService = emailService;
            _accountRepository = accountRepository;
            _firebaseService = firebaseService;
            _logger = logger;

        }

        public async Task<List<Participant>> GetParticipantsByEventId(Guid eventId)
        {
            return await _participantRepository.GetByEventIdAsync(eventId);
        }

        private async Task<string> GenerateAndUploadQRCodeAsync(string qrData, string fileName)
        {
            try
            {
                _logger.LogInformation("Starting QR code generation for data: {QrData}", qrData);

                // Generate QR code
                var qrGenerator = new QRCodeGenerator();
                var qrCodeData = qrGenerator.CreateQrCode(qrData, QRCodeGenerator.ECCLevel.Q);
                var qrCode = new QRCode(qrCodeData);
                using var qrBitmap = qrCode.GetGraphic(20);

                // Save QR code to a memory stream
                using var memoryStream = new MemoryStream();
                qrBitmap.Save(memoryStream, ImageFormat.Png);
                memoryStream.Seek(0, SeekOrigin.Begin);

                _logger.LogInformation("QR code generated successfully. Preparing to upload to Firebase.");

                // Convert memory stream to IFormFile for Firebase upload
                var formFile = new FormFile(memoryStream, 0, memoryStream.Length, "qrCode", fileName)
                {
                    Headers = new HeaderDictionary(),
                    ContentType = "image/png"
                };

                // Upload QR code to Firebase
                var qrCodeUrl = await _firebaseService.UploadFile(formFile);

                _logger.LogInformation("QR code uploaded successfully. URL: {QrCodeUrl}", qrCodeUrl);

                return qrCodeUrl;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating or uploading QR code. Data: {QrData}, FileName: {FileName}", qrData, fileName);
                throw new InvalidOperationException("Failed to generate or upload QR code.", ex);
            }
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

            // Generate QR code and upload to Firebase
            var qrCodeDataString = $"AccountId={accountId}&ParticipantId={participant.Id}&EventId={eventId}";
            var qrCodeUrl = await GenerateAndUploadQRCodeAsync(qrCodeDataString, $"qrcode_{participant.Id}.png");

            // Create email body with QR code link
            var emailBody = $@"
                <h2>Đăng ký sự kiện thành công</h2>
                <p>Cảm ơn bạn đã đăng ký tham gia sự kiện <strong>{eventItem.Title}</strong>.</p>
                <p>Vui lòng sử dụng QR code dưới đây để check-in tại sự kiện:</p>
                <img src='{qrCodeUrl}' alt='QR Code' style='width:200px;height:200px;' />
                <p>Nếu không thấy hình ảnh, bạn có thể tải QR code tại: <a href='{qrCodeUrl}'>Tải QR Code</a></p>
                <p>Thông tin:<br/>
                   - Account ID: {accountId}<br/>
                   - Participant ID: {participant.Id}<br/>
                   - Event ID: {eventId}</p>";

            // Send email
            var emailSent = await _emailService.SendEmailAsync(account.Email, "Xác nhận đăng ký sự kiện", emailBody);
            if (!emailSent)
            {
                _logger.LogWarning("Failed to send email to {Email}", account.Email);
            }

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
                FullName = a.FullName,
                ProfilePicture = a.ProfilePicture
            }).ToList();
        }
    }
}