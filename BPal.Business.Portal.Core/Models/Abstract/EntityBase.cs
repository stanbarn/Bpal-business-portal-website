using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace BPal.Business.Portal.Core.Models
{
    public abstract class EntityBase
    {
        [Required]
        public DateTime CreatedOn { get; set; }

        // format: accountId@subscription
        [Required]
        [StringLength(255)]
        public string CreatedBy { get; set; }
    }
}
