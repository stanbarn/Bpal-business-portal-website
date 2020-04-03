using BPal.Business.Portal.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BPal.Business.Portal.Core.Services
{
    public interface ICollectionService
    {
        Task<ServiceResponse<CollectionListResponse>> GetAsync(string pageNumber, string pageSize);
        Task<ServiceResponse<Collection>> GetAsync(string collectionId);
        Task<ServiceResponse<Collection>> CreateAsync(Collection collection);
        Task<ServiceResponse<Collection>> UpdateAsync(Collection collection);
        Task<ServiceResponse<Collection>> RemoveAsync(Collection collection);
    }
}
