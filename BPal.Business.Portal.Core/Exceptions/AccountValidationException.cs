using BPal.Business.Portal.Core.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace BPal.Business.Portal.Core.Exceptions
{
    public class AccountValidationException : Exception
    {
        public AccountValidationException() { }
        public AccountValidationException(string message) : base(message) { }
        public AccountValidationException(SimpleMessage simpleMessage) : base(JsonConvert.SerializeObject(simpleMessage)) { }
        public AccountValidationException(string message, Exception inner) : base(message, inner) { }
        public AccountValidationException(SimpleMessage simpleMessage, Exception inner) : base(JsonConvert.SerializeObject(simpleMessage), inner) { }
        protected AccountValidationException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
