using Eventa_BusinessObject.Entities;
using Eventa_Repositories.Interfaces;
using Eventa_Services.Interfaces;
using Google.Apis.Util;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eventa_Services.Implements
{
    public class CheckInService : ICheckInService
    {
        private readonly ICheckInRepository _checkInRepository;
        private readonly IParticipantRepository _participantRepository;
        private readonly IEventRepository _eventRepository;
        private readonly ITicketRepository _ticketRepository;
        private readonly IMongoClient _mongoClient;

        public CheckInService(ICheckInRepository checkInRepository, IParticipantRepository participantRepository,
                              IEventRepository eventRepository, ITicketRepository ticketRepository, IMongoClient mongoClient)
        {
            _checkInRepository = checkInRepository;
            _participantRepository = participantRepository;
            _eventRepository = eventRepository;
            _ticketRepository = ticketRepository;
            _mongoClient = mongoClient;
        }

        /// <summary>
        /// Check-in một participant vào sự kiện.
        /// </summary>
        /// <param name="participantId">ID của participant</param>
        /// <param name="eventId">ID của sự kiện</param>
        /// <returns>Task<bool> - True nếu check-in thành công, False nếu thất bại</returns>
        /// <exception cref="InvalidOperationException">Ném ra khi có lỗi logic</exception>
        public async Task<bool> CheckInParticipant(Guid participantId, Guid eventId)
        {
            //Kiểm tra điều kiện check-in trước khi vào transaction
            //1. Check participant 
            var participant = await _participantRepository.GetAsync(participantId);
            if (participant == null)
                throw new InvalidOperationException($"Participant với ID {participantId} không tồn tại.");

            //2 . Check event
            var eventItem = await _eventRepository.GetById(eventId);
            if (eventItem == null)
                throw new InvalidOperationException($"Sự kiện với ID {eventId} không tồn tại.");
            if (eventItem.StartDate > DateTime.UtcNow)
                throw new InvalidOperationException($"Sự kiện {eventItem.Title} chưa bắt đầu.");
            if (eventItem.EndDate < DateTime.UtcNow)
                throw new InvalidOperationException($"Sự kiện {eventItem.Title} đã kết thúc.");

            //3 . Check ticket
            var ticket = await _ticketRepository.GetTicketsByParticipantIdAsync(participantId);
            var validTicket = ticket.FirstOrDefault(t => t.EventId == eventId && !t.IsUsed);
            if (validTicket == null)
                throw new InvalidOperationException($"Participant {participant.AccountId} không có vé tham gia sự kiện {eventItem.Title} hoặc vé đã được sử dụng.");
            //Bắt đầu transaction 
            using (var session = await _mongoClient.StartSessionAsync())
            {
                session.StartTransaction();
                try
                {
                    //4. Create check-in and update status of ticket
                    var newCheckIn = new CheckIn
                    {
                        Id = Guid.NewGuid(),
                        EventId = eventId,
                        ParticipantId = participantId,
                        CheckInTime = DateTime.UtcNow
                    };

                    validTicket.IsUsed = true;
                    var ticketUpdate = await _ticketRepository.UpdateAsync(validTicket);
                    if (!ticketUpdate)
                        throw new InvalidOperationException($"Không thể cập nhật trạng thái vé.");

                    var checkInSuccess = await _checkInRepository.AddAsync(newCheckIn);
                    if (!checkInSuccess)
                        throw new InvalidOperationException($"Không thể thêm check-in.");

                    //5. Update participant's point and return result
                    participant.IsCheckedIn = true;
                    var participantUpdate = await _participantRepository.Update(participant);
                    if (!participantUpdate)
                        throw new InvalidOperationException($"Không thể cập nhật trạng thái check-in của participant.");

                    // 6. Commit transaction nếu mọi thứ thành công
                    await session.CommitTransactionAsync();
                    return true;
                }
                catch (Exception ex)
                {
                    // 7. Rollback transaction nếu có lỗi
                    await session.AbortTransactionAsync();
                    throw new InvalidOperationException($"Check-in thất bại: {ex.Message}", ex);
                }
            }
        }

        /// <summary>
        /// Check-in một participant bằng QR code chứa AccountId, ParticipantId, EventId.
        /// </summary>
        /// <param name="qrCodeData">Dữ liệu QR code (dạng "AccountId=...&ParticipantId=...&EventId=...")</param>
        /// <returns>Task<bool> - True nếu check-in thành công, False nếu thất bại</returns>
        /// <exception cref="InvalidOperationException">Ném ra khi dữ liệu QR không hợp lệ hoặc check-in thất bại</exception>
        public async Task<bool> CheckInByQRCode(string qrCodeData)
        {
            // 1. Parse dữ liệu từ QR code
            if (string.IsNullOrEmpty(qrCodeData))
                throw new InvalidOperationException("Dữ liệu QR code không hợp lệ.");

            var qrParams = qrCodeData.Split('&')
                .Select(p => p.Split('='))
                .ToDictionary(p => p[0], p => p[1]);

            if (!qrParams.TryGetValue("AccountId", out var accountIdStr) ||
                !qrParams.TryGetValue("ParticipantId", out var participantIdStr) ||
                !qrParams.TryGetValue("EventId", out var eventIdStr) ||
                !Guid.TryParse(accountIdStr, out Guid accountId) ||
                !Guid.TryParse(participantIdStr, out Guid participantId) ||
                !Guid.TryParse(eventIdStr, out Guid eventId))
            {
                throw new InvalidOperationException("Dữ liệu QR code không đúng định dạng hoặc thiếu thông tin.");
            }

            // 2. Kiểm tra participant
            var participant = await _participantRepository.GetAsync(participantId);
            if (participant == null || participant.AccountId != accountId || participant.EventId != eventId)
                throw new InvalidOperationException("Thông tin participant không khớp với dữ liệu QR code.");

            // 3. Kiểm tra sự kiện
            var eventItem = await _eventRepository.GetById(eventId);
            if (eventItem == null)
                throw new InvalidOperationException($"Sự kiện với ID {eventId} không tồn tại.");
            if (eventItem.StartDate > DateTime.UtcNow)
                throw new InvalidOperationException($"Sự kiện {eventItem.Title} chưa bắt đầu.");
            if (eventItem.EndDate < DateTime.UtcNow)
                throw new InvalidOperationException($"Sự kiện {eventItem.Title} đã kết thúc.");

            // 4. Kiểm tra vé
            var tickets = await _ticketRepository.GetTicketsByParticipantIdAsync(participantId);
            var validTicket = tickets.FirstOrDefault(t => t.EventId == eventId && !t.IsUsed);
            if (validTicket == null)
                throw new InvalidOperationException($"Participant {participant.AccountId} không có vé hợp lệ cho sự kiện {eventItem.Title} hoặc vé đã được sử dụng.");

            // 5. Kiểm tra xem đã check-in chưa
            var existingCheckIn = await _checkInRepository.GetByParticipantAndEventAsync(participantId, eventId);
            if (existingCheckIn != null)
                throw new InvalidOperationException($"Participant với ID {participantId} đã check-in cho sự kiện {eventId}.");

            // Bắt đầu transaction
            using (var session = await _mongoClient.StartSessionAsync())
            {
                session.StartTransaction();
                try
                {
                    // 6. Tạo check-in
                    var newCheckIn = new CheckIn
                    {
                        Id = Guid.NewGuid(),
                        EventId = eventId,
                        ParticipantId = participantId,
                        CheckInTime = DateTime.UtcNow
                    };
                    var checkInSuccess = await _checkInRepository.AddAsync(newCheckIn);
                    if (!checkInSuccess)
                        throw new InvalidOperationException("Không thể thêm check-in.");

                    // 7. Cập nhật trạng thái vé
                    validTicket.IsUsed = true;
                    var ticketUpdate = await _ticketRepository.UpdateAsync(validTicket);
                    if (!ticketUpdate)
                        throw new InvalidOperationException("Không thể cập nhật trạng thái vé.");

                    // 8. Cập nhật trạng thái participant
                    participant.IsCheckedIn = true;
                    var participantUpdate = await _participantRepository.Update(participant);
                    if (!participantUpdate)
                        throw new InvalidOperationException("Không thể cập nhật trạng thái check-in của participant.");

                    // Commit transaction
                    await session.CommitTransactionAsync();
                    return true;
                }
                catch (Exception ex)
                {
                    // Rollback nếu có lỗi
                    await session.AbortTransactionAsync();
                    throw new InvalidOperationException($"Check-in bằng QR code thất bại: {ex.Message}", ex);
                }
            }
        }
    }
}
