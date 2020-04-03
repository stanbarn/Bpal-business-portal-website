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
        Task<ServiceResponse<Account>> GetAsync(string AccountId);

        Task<ServiceResponse<Account>> UpdateAsync(Account Account);

        Task<ServiceResponse<Account>> CreateAsync(Account account);

        Task<ServiceResponse<Account>> GetByEmailAsync(string email);

        Task<TokenResponse> AuthenticateAsync(string username, string password);

    }
}
