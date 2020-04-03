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
    public class CollectionService : ICollectionService
    {
        ILogger<CollectionService> _logger;
        IRestClient _restClient;
        IRestRequest _restRequest;

        public CollectionService(ILogger<CollectionService> logger)
        {
            _logger = logger;
            _restClient = new RestClient(AppSettings.BplasApiBaseUrl);
        }

        public async Task<ServiceResponse<Collection>> CreateAsync(Collection collection)
        {
            _restRequest = new RestRequest(Method.POST);
            _restRequest.Resource = "Collections/forms";
            _restRequest.AddJsonBody(collection);
            var response = await _restClient.ExecuteAsync<ServiceResponse<Collection>>(_restRequest);
            return response?.Data;
        }

        public async Task<ServiceResponse<CollectionListResponse>> GetAsync(string pageNumber = "1", string pageSize = "10")
        {
            _restRequest = new RestRequest(Method.GET);
            _restRequest.Resource = "Collections/forms?pageSize={pageSize}&pageNumber={pageNumber}";
            _restRequest.AddUrlSegment("pageNumber", pageNumber);
            _restRequest.AddUrlSegment("pageSize", pageSize);
            var response = await _restClient.ExecuteAsync<ServiceResponse<CollectionListResponse>>(_restRequest);
            return response?.Data;
        }

        public async Task<ServiceResponse<Collection>> GetAsync(string collectionId)
        {
            _restRequest = new RestRequest(Method.GET);
            _restRequest.Resource = "Collections/forms/{formId}";
            _restRequest.AddUrlSegment("formId", collectionId);
            var response = await _restClient.ExecuteAsync<ServiceResponse<Collection>>(_restRequest);
            return response?.Data;
        }

        public async Task<ServiceResponse<Collection>> RemoveAsync(Collection collection)
        {
            _restRequest = new RestRequest(Method.DELETE);
            _restRequest.Resource = "Collections/forms/{formId}";
            _restRequest.AddUrlSegment("formId", collection?.Id);
            _restRequest.AddJsonBody(collection);
            var response = await _restClient.ExecuteAsync<ServiceResponse<Collection>>(_restRequest);
            return response?.Data;
        }

        public async Task<ServiceResponse<Collection>> UpdateAsync(Collection collection)
        {
            _restRequest = new RestRequest(Method.PUT);
            _restRequest.Resource = "Collections/forms";
            _restRequest.AddJsonBody(collection);
            var response = await _restClient.ExecuteAsync<ServiceResponse<Collection>>(_restRequest);
            return response?.Data;
        }
    }
}
