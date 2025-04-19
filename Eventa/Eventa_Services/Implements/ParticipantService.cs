using Eventa_BusinessObject.DTOs.Account;
using Eventa_BusinessObject.Entities;
using Eventa_Repositories.Implements;
using Eventa_Repositories.Interfaces;
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
using Microsoft.Extensions.Configuration;
using Eventa_Services.Util;

namespace Eventa_Services.Implement
{
    public class ParticipantService : IParticipantService
    {
        private readonly IParticipantRepository _participantRepository;
        private readonly IEventRepository _eventRepository;
        private readonly IEmailService _emailService;
        private readonly IAccountRepository _accountRepository;
        private readonly ILogger<ParticipantService> _logger;
        private readonly IConfiguration _configuration;

        public ParticipantService(
            IParticipantRepository participantRepository,
            IEventRepository eventRepository,
            IEmailService emailService,
            IAccountRepository accountRepository,
            ILogger<ParticipantService> logger,
            IConfiguration configuration)
        {
            _participantRepository = participantRepository;
            _eventRepository = eventRepository;
            _emailService = emailService;
            _accountRepository = accountRepository;
            _logger = logger;
            _configuration = configuration;
        }

        public async Task<List<Participant>> GetParticipantsByEventId(Guid eventId)
        {
            return await _participantRepository.GetByEventIdAsync(eventId);
        }


        public async Task<bool> RegisterParticipant(Guid accountId, Guid eventId)
        {
            try
            {
                var account = await _accountRepository.GetAsync(accountId);
                if (account == null)
                    throw new InvalidOperationException("Tài khoản không tồn tại.");
                var eventItem = await _eventRepository.GetById(eventId);
                if (eventItem == null)
                    throw new InvalidOperationException("Sự kiện không tồn tại.");
                var existingParticipant = await _participantRepository.GetByAccountIdAsync(accountId);
                if (existingParticipant != null)
                    throw new InvalidOperationException("Tài khoản đã tham gia sự kiện này.");
                var participant = new Participant
                {
                    AccountId = accountId,
                    EventId = eventId,
                    IsConfirmed = false,
                    IsCheckedIn = false
                };
                participant.UniqueCode = new Random().Next(100000, 999999).ToString();
                var emailBody = $@"
                                <html>
                                <head>
                                    <style>
                                        body {{
                                            font-family: Arial, sans-serif;
                                            color: #333333;
                                            background-color: #f4f4f9;
                                            padding: 20px;
                                        }}
                                        .email-container {{
                                            width: 100%;
                                            max-width: 600px;
                                            margin: 0 auto;
                                            background-color: #ffffff;
                                            border-radius: 8px;
                                            padding: 20px;
                                            box-shadow: 0px 4px 8px rgba(0, 0, 0, 0.1);
                                        }}
                                        .header {{
                                            text-align: center;
                                            font-size: 24px;
                                            color: #4CAF50;
                                        }}
                                        .content {{
                                            margin-top: 20px;
                                            font-size: 16px;
                                            line-height: 1.6;
                                        }}
                                        .info {{
                                            margin-top: 10px;
                                            padding: 10px;
                                            background-color: #f9f9f9;
                                            border: 1px solid #e0e0e0;
                                            border-radius: 5px;
                                        }}
                                        .info p {{
                                            margin: 5px 0;
                                        }}
                                        .code {{
                                            font-weight: bold;
                                            font-size: 18px;
                                            color: #FF5722;
                                        }}
                                        .footer {{
                                            margin-top: 30px;
                                            text-align: center;
                                            font-size: 14px;
                                            color: #777777;
                                        }}
                                    </style>
                                </head>
                                <body>
                                    <div class='email-container'>
                                        <div class='header'>
                                            Đăng ký tham gia sự kiện thành công
                                        </div>
                                        <div class='content'>
                                            <p>Cảm ơn bạn đã đăng ký tham gia sự kiện <strong>{eventItem.Title}</strong>.</p>
                                            <p>Vui lòng sử dụng thông tin dưới đây để check-in tại sự kiện:</p>
            
                                            <div class='info'>
                                                <p><strong>Thông tin:</strong></p>
                                                <p>- Account ID: {accountId}</p>
                                                <p>- Participant ID: {participant.Id}</p>
                                                <p>- Event ID: {eventId}</p>
                                            </div>
            
                                            <p>Mã xác nhận của bạn là:</p>
                                            <p class='code'>{participant.UniqueCode}</p>
                                        </div>
                                        <div class='footer'>
                                            <p>Cảm ơn bạn đã tham gia sự kiện của chúng tôi!</p>
                                        </div>
                                    </div>
                                </body>
                                </html>";
                var emailSubject = "Đăng ký tham gia sự kiện thành công";
                var emailSent = await _emailService.SendEmailAsync(account.Email, emailSubject, emailBody);
                if (!emailSent)
                    throw new InvalidOperationException("Không thể gửi email xác nhận.");
                var result = await _participantRepository.AddAsync(participant);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while registering participant.");
                throw new InvalidOperationException("Đã xảy ra lỗi trong quá trình đăng ký tham gia sự kiện.");
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
        public async Task<List<Guid>> GetAllEventParticipantedOfMe(HttpContext httpContext)
        {
            var accountId = UserUtil.GetAccountId(httpContext);
            var participants = await _participantRepository.GetAllEventParticipantedOfMe((Guid)accountId);
            return participants;
        }
    }
}