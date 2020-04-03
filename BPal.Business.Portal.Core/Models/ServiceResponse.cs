using System;
using System.Collections.Generic;
using System.Text;

namespace BPal.Business.Portal.Core.Models
{
    public class ServiceResponse
    {
        public long Code { get; set; }

        public string Message { get; set; }

        public object Data { get; set; }
    }

    public class ServiceResponse<T>
    {
        public long Code { get; set; }

        public string Message { get; set; }

        public T Data { get; set; }
    }
}
