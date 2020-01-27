using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace BPal.Business.Portal.Website.Controllers
{
    public class CollectionsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}