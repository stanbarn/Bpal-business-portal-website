using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace BPal.Business.Portal.Core.Models
{
    /// <summary>
    /// Describable entity interface
    /// </summary>
    public interface IDescribable
    {

        [StringLength(512)]
        string ShortDescription { get; set; }

        string LongDescription { get; set; }
    }
}
