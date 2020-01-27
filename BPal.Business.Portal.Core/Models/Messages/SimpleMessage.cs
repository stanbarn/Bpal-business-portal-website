using BPal.Business.Portal.Core.Enums;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace BPal.Business.Portal.Core.Models
{
    public class SimpleMessage
    {
        public SimpleMessage() { }

        public SimpleMessage(int code, string message)
        {
            Code = code;
            Message = message;
        }

        public SimpleMessage(int code, string message, MessageLevel level)
        {
            Code = code;
            Message = message;
            Level = level;
        }

        public SimpleMessage(int code, string message, params object[] @params)
        {
            Code = code;
            Message = string.Format(message, @params);
        }

        public SimpleMessage(int code, MessageLevel level, string message, params object[] @params)
        {
            Code = code;
            Message = string.Format(message, @params);
            Level = level;
        }

        public int Code { get; set; }

        public string Message { get; set; }

        public MessageLevel Level { get; set; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
