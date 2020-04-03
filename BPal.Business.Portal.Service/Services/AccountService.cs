using BPal.Business.Portal.Core.Models;
using BPal.Business.Portal.Core.Services;
using BPal.Business.Portal.Core.Settings;
using Microsoft.Extensions.Logging;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPal.Business.Portal.Service.Services
{
    public class AccountService : IAccountService
    {
        ILogger<AccountService> _logger;
        IRestClient _restClient;
        IRestRequest _restRequest;

        public AccountService(ILogger<AccountService> logger)
        {
            _logger = logger;
            _restClient = new RestClient(AppSettings.BplasApiBaseUrl);
        }

        public  async Task<TokenResponse> AuthenticateAsync(string username, string password)
        {
            _restRequest = new RestRequest(Method.POST);
            _restRequest.Resource = "auth/login";
            _restRequest.AddJsonBody(new
            {
                username = username,
                password = password
            });
            var response = await _restClient.ExecuteAsync<TokenResponse>(_restRequest);
            return response?.Data;
        }

        public async Task<ServiceResponse<Account>> CreateAsync(Account account)
        {
            _restRequest = new RestRequest(Method.POST);
            _restRequest.Resource = "auth/login";
            _restRequest.AddJsonBody(account);
            var response = await _restClient.ExecuteAsync<ServiceResponse<Account>>(_restRequest);
            return response?.Data;
        }

        public async Task<ServiceResponse<Account>> GetAsync(string AccountId)
        {
            _restRequest = new RestRequest(Method.GET);
            _restRequest.Resource = "accounts/{AccountId}";
            _restRequest.AddUrlSegment("AccountId", AccountId);
            var response = await _restClient.ExecuteAsync<ServiceResponse<Account>>(_restRequest);
            return response?.Data;
        }

        public Task<ServiceResponse<Account>> GetByEmailAsync(string email)
        {
            throw new NotImplementedException();
        }

        public Task<ServiceResponse<Account>> UpdateAsync(Account Account)
        {
            throw new NotImplementedException();
        }
    }
}
