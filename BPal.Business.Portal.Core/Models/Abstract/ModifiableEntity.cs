using System;
using System.Collections.Generic;
using System.Text;

namespace BPal.Business.Portal.Core.Models
{
    /// <summary>
    /// Base class for all modifiable entities
    /// </summary>
    public abstract class ModifiableEntity : EntityBase
    {
        public ModifiableEntity()
        {
            ModifiedOn = DateTime.UtcNow;
        }

        public DateTime ModifiedOn { get; set; }

        // format: accountId@subscription
        public string ModifiedBy { get; set; }
    }
}
