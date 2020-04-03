using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BPal.Business.Portal.Core.Services;
using Microsoft.AspNetCore.Mvc;

namespace BPal.Business.Portal.Website.Controllers
{
    public class CollectionsController : Controller
    {
        ICollectionService _collectionService;

        public CollectionsController(ICollectionService collectionService)
        {
            _collectionService = collectionService;
        }

        public async Task<IActionResult> Index()
        {
            return View();
        }

        [HttpGet("collections/forms")]
        public async Task<JsonResult> Get(int start = 0, int length = 10)
        {
            int pageNumber = 1;

            if (start > 0)
                pageNumber = (start / length) + 1;

            var collectionResponse = await _collectionService.GetAsync(pageNumber.ToString(), length.ToString());
            return Json(collectionResponse.Data);
        }

        [HttpGet("collections/forms/{formId}")]
        public async Task<IActionResult> Get(string formId)
        {
            var collectionResponse = await _collectionService.GetAsync(formId);
            return PartialView("CollectionFormDetailsPartial", collectionResponse.Data);
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