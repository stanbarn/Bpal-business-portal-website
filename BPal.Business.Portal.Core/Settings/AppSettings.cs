using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace BPal.Business.Portal.Core.Settings
{
    public static class AppSettings
    {
        private static IConfiguration _configuration => new ConfigurationBuilder()
           .SetBasePath(AppContext.BaseDirectory)

           // .AddJsonFile($"appsettings.json", optional: false, reloadOnChange: false)
           .AddJsonFile($"appsettings.Development.json", optional: false, reloadOnChange: false)
           .Build();


        public static string AuthSecretKey
            => _configuration["AuthSecretKey"];

        public static string BplasApiBaseUrl
            => _configuration["BplasApiBaseUrl"];
    }
}
