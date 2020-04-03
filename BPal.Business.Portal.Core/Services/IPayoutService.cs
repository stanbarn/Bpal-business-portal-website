using BPal.Business.Portal.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BPal.Business.Portal.Core.Services
{
    public interface IPayoutService
    {
        Task<ServiceResponse<PayoutListResponse>> GetAsync(string pageNumber, string pageSize);
        Task<ServiceResponse<Payout>> GetAsync(string collectionId);
        Task<ServiceResponse<Payout>> CreateAsync(Payout payout);
        Task<ServiceResponse<Payout>> UpdateAsync(Payout payout);
        Task<ServiceResponse<Payout>> RemoveAsync(Payout payout);
    }
}
