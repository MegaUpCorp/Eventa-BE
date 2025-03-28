﻿using Eventa_BusinessObject.DTOs.Account;
using Eventa_BusinessObject.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eventa_Repositories.Interfaces
{
    public interface IAccountRepository : IRepository<Account>
    {
        Task<bool> AddAsync(Account entity, CancellationToken cancellationToken = default);
        Task<Account?> GetAccountByEmailAsync(string email);
        Task<Account?> GetAccountByUsernameAsync(string username);
        Task<Account?> GetAccountByPhoneNumberAsync(string phoneNumber);
        Task<bool> AddCalendarAsync(Calendar calendar, CancellationToken cancellationToken = default);
       // Task<List<Calendar>> GetCalendarsByAccountIdAsync(Guid accountId, CancellationToken cancellationToken = default);
        Task<List<Calendar>> GetAllCalendarsAsync(CancellationToken cancellationToken = default);
        Task<Calendar?> GetCalendarByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<Calendar?> GetCalendarByPublicUrlAsync(string publicUrl);
        Task<CalendarDTO?> GetCalendarByPublicUrlAsync1(string publicUrl, Guid accountId);

        Task<AccountDTO> GetBasicAccountByOrganizerId(Guid accountID, CancellationToken cancellationToken = default);
        Task<List<Calendar>> GetCalendarsByAccountID(Guid accountID, CancellationToken cancellationToken = default);
        Task<List<Calendar>> GetCalendarsNotMe(Guid accountID, CancellationToken cancellationToken = default);
        Task<bool> SubscribeCalendar(Guid accountId, string url);
        Task<bool> UpdateCalendar(Calendar calendar);
    }
}
