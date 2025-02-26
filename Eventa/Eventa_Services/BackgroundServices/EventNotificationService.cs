using Eventa_BusinessObject.Entities;
using Eventa_Repositories.Interfaces;
using Eventa_Services.Implements;
using Eventa_Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Eventa_Services.BackgroundServices
{
    public class EventNotificationService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;

        public EventNotificationService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    // Tạo một scope mới để truy cập scoped services
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var eventRepository = scope.ServiceProvider.GetRequiredService<IEventRepository>();
                        var participantRepository = scope.ServiceProvider.GetRequiredService<IParticipantRepository>();
                        var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();
                        var accountRepository = scope.ServiceProvider.GetRequiredService<IAccountRepository>();

                        var now = DateTime.UtcNow;
                        var events = await eventRepository.GetAll();
                        var upcomingEvents = events.Where(e => !e.DelFlg && e.StartDate > now);

                        foreach (var evt in upcomingEvents)
                        {
                            var timeDifference = evt.StartDate - now;
                            if (timeDifference.TotalMinutes <= 30 && timeDifference.TotalMinutes > 29) // 30 phút trước
                            {
                                var participants = await participantRepository.GetByEventIdAsync(evt.Id);
                                foreach (var participant in participants)
                                {
                                    var account = await accountRepository.GetAsync(participant.AccountId);
                                    if (account != null && !string.IsNullOrEmpty(account.Email))
                                    {
                                        await emailService.SendEmailAsync(
                                            account.Email,
                                            $"Nhắc nhở: Sự kiện {evt.Title} sắp bắt đầu!",
                                            $"Sự kiện {evt.Title} sẽ diễn ra lúc {evt.StartDate.ToLocalTime()}. Hãy chuẩn bị nhé!"
                                        );
                                    }
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"❌ Lỗi trong EventNotificationService: {ex.Message}");
                }

                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken); // Kiểm tra mỗi phút
            }
        }
    }
}