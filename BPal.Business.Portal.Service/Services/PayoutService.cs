using BPal.Business.Portal.Core.Models;
using BPal.Business.Portal.Core.Services;
using BPal.Business.Portal.Core.Settings;
using Microsoft.Extensions.Logging;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BPal.Business.Portal.Service.Services
{
    public class PayoutService : IPayoutService
    {
        ILogger<PayoutService> _logger;
        IRestClient _restClient;
        IRestRequest _restRequest;

        public PayoutService(ILogger<PayoutService> logger)
        {
            _logger = logger;
            _restClient = new RestClient(AppSettings.BplasApiBaseUrl);
        }

        public async Task<ServiceResponse<Payout>> CreateAsync(Payout payout)
        {
            _restRequest = new RestRequest(Method.POST);
            _restRequest.Resource = "Payouts/forms";
            _restRequest.AddJsonBody(payout);
            var response = await _restClient.ExecuteAsync<ServiceResponse<Payout>>(_restRequest);
            return response?.Data;
        }

        public async Task<ServiceResponse<PayoutListResponse>> GetAsync(string pageNumber = "1", string pageSize = "10")
        {
            _restRequest = new RestRequest(Method.GET);
            _restRequest.Resource = "Payouts/forms?pageSize={pageSize}&pageNumber={pageNumber}";
            _restRequest.AddUrlSegment("pageNumber", pageNumber);
            _restRequest.AddUrlSegment("pageSize", pageSize);
            var response = await _restClient.ExecuteAsync<ServiceResponse<PayoutListResponse>>(_restRequest);
            return response?.Data;
        }

        public async Task<ServiceResponse<Payout>> GetAsync(string payoutId)
        {
            _restRequest = new RestRequest(Method.GET);
            _restRequest.Resource = "Payouts/forms/{formId}";
            _restRequest.AddUrlSegment("formId", payoutId);
            var response = await _restClient.ExecuteAsync<ServiceResponse<Payout>>(_restRequest);
            return response?.Data;
        }

        public async Task<ServiceResponse<Payout>> RemoveAsync(Payout payout)
        {
            _restRequest = new RestRequest(Method.DELETE);
            _restRequest.Resource = "Payouts/forms";
            _restRequest.AddJsonBody(payout);
            var response = await _restClient.ExecuteAsync<ServiceResponse<Payout>>(_restRequest);
            return response?.Data;
        }

        public async Task<ServiceResponse<Payout>> UpdateAsync(Payout payout)
        {
            _restRequest = new RestRequest(Method.PUT);
            _restRequest.Resource = "Payouts/forms";
            _restRequest.AddJsonBody(payout);
            var response = await _restClient.ExecuteAsync<ServiceResponse<Payout>>(_restRequest);
            return response?.Data;
        }
    }
}
