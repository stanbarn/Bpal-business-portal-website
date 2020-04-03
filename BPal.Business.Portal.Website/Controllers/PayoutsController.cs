using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BPal.Business.Portal.Core.Services;
using Microsoft.AspNetCore.Mvc;

namespace BPal.Business.Portal.Website.Controllers
{
    public class PayoutsController : Controller
    {
        IPayoutService _payoutService;

        public PayoutsController(IPayoutService payoutService)
        {
            _payoutService = payoutService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet("payouts/forms")]
        public async Task<JsonResult> Get(int start = 0, int length = 10)
        {
            int pageNumber = 1;

            if (start > 0)
                pageNumber = (start / length) + 1;

            var collectionResponse = await _payoutService.GetAsync(pageNumber.ToString(), length.ToString());
            return Json(collectionResponse.Data);
        }

        [HttpGet("payouts/forms/{formId}")]
        public async Task<IActionResult> Get(string formId)
        {
            var payoutResponse = await _payoutService.GetAsync(formId);
            return PartialView("PayoutFormDetailsPartial", payoutResponse.Data);
        }

        public async Task<IActionResult> Add()
        {
            return View();
        }

        public async Task<IActionResult> Edit()
        {
            return View();
        }
        public async Task<IActionResult> Remove()
        {
            return View();
        }
    }
}