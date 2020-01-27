using System;
using System.Collections.Generic;
using System.Text;

namespace BPal.Business.Portal.Core.Enums
{
    public enum PaymentStatus
    {
        NOT_SET = 0,
        LOGGED = 1,
        PROCESSING = 2,
        SUCCESSFUL = 3,
        FAILED = 4,
        UNKNOWN = 5
    }
}
