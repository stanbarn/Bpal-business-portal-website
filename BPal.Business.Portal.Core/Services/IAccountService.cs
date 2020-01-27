using BPal.Business.Portal.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPal.Business.Portal.Core.Services
{
    public interface IAccountService
    {
        IQueryable<Account> Get();

        Task<Account> GetAsync(string AccountId);

        Task<Account> UpdateAsync(Account Account);

        Task<Account> CreateAsync(Account account);

        Task<Account> GetByEmailAsync(string email);

        Task<Account> ActivateAccountAsync(string email, string auth);

        Task<Account> AuthenticateAsync(string username, string password);

        Task<bool> IsEmailExistsAsync(string emailAddress);

        Task<bool> IsTenantEmailExistsAsync(string emailAddress);

        Task<Account> DeleteAsync(string Id);
    }
}
