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
        public async Task<bool> CheckInParticipant(Guid participantId, Guid eventId, string uniqueCode)
        {
            // Kiểm tra participant
            var participant = await _participantRepository.GetAsync(participantId);
            if (participant == null)
                throw new InvalidOperationException($"Participant với ID {participantId} không tồn tại.");

            // Kiểm tra mã UniqueCode
            if (participant.UniqueCode != uniqueCode)
                throw new InvalidOperationException("Mã UniqueCode không hợp lệ.");

            // Kiểm tra sự kiện
            var eventItem = await _eventRepository.GetById(eventId);
            if (eventItem == null)
                throw new InvalidOperationException($"Sự kiện với ID {eventId} không tồn tại.");
            if (eventItem.StartDate > DateTime.UtcNow)
                throw new InvalidOperationException($"Sự kiện {eventItem.Title} chưa bắt đầu.");
            if (eventItem.EndDate < DateTime.UtcNow)
                throw new InvalidOperationException($"Sự kiện {eventItem.Title} đã kết thúc.");

            // Kiểm tra vé
            var ticket = await _ticketRepository.GetTicketsByParticipantIdAsync(participantId);
            var validTicket = ticket.FirstOrDefault(t => t.EventId == eventId && !t.IsUsed);
            if (validTicket == null)
                throw new InvalidOperationException($"Participant {participant.AccountId} không có vé hợp lệ cho sự kiện {eventItem.Title} hoặc vé đã được sử dụng.");

            // Bắt đầu transaction
            using (var session = await _mongoClient.StartSessionAsync())
            {
                session.StartTransaction();
                try
                {
                    // Tạo check-in
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

                    // Cập nhật trạng thái vé
                    validTicket.IsUsed = true;
                    var ticketUpdate = await _ticketRepository.UpdateAsync(validTicket);
                    if (!ticketUpdate)
                        throw new InvalidOperationException("Không thể cập nhật trạng thái vé.");

                    // Cập nhật trạng thái participant
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
                    throw new InvalidOperationException($"Check-in thất bại: {ex.Message}", ex);
                }
            }
        }

    }
}
