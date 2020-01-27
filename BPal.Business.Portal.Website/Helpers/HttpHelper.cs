using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BPal.Business.Portal.Website.Helpers
{
    public static class HttpHelper
    {
        private static IHttpContextAccessor _contextAccessor;
        public static void Configure(IHttpContextAccessor accessor)
        {
            _contextAccessor = accessor;
        }

        public static HttpContext HttpContext => _contextAccessor.HttpContext;
    }
}
