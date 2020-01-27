using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace BPal.Business.Portal.Website.Controllers
{
    public class CollectionsController : Controller
    {
        public async Task<IActionResult> Index()
        {
            return View();
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