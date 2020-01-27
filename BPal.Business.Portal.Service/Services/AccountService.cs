using BPal.Business.Portal.Core.Models;
using BPal.Business.Portal.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPal.Business.Portal.Service.Services
{
    public class AccountService : IAccountService
    {
        public Task<Account> ActivateAccountAsync(string email, string auth)
        {
            throw new NotImplementedException();
        }

        public Task<Account> AuthenticateAsync(string username, string password)
        {
            throw new NotImplementedException();
        }

        public Task<Account> CreateAsync(Account account)
        {
            throw new NotImplementedException();
        }

        public Task<Account> DeleteAsync(string Id)
        {
            throw new NotImplementedException();
        }

        public IQueryable<Account> Get()
        {
            throw new NotImplementedException();
        }

        public Task<Account> GetAsync(string AccountId)
        {
            throw new NotImplementedException();
        }

        public Task<Account> GetByEmailAsync(string email)
        {
            throw new NotImplementedException();
        }

        public Task<bool> IsEmailExistsAsync(string emailAddress)
        {
            throw new NotImplementedException();
        }

        public Task<bool> IsTenantEmailExistsAsync(string emailAddress)
        {
            throw new NotImplementedException();
        }

        public Task<Account> UpdateAsync(Account Account)
        {
            throw new NotImplementedException();
        }
    }
}
