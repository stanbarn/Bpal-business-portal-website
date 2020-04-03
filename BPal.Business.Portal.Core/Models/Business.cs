using System;
using System.Collections.Generic;
using System.Text;

namespace BPal.Business.Portal.Core.Models
{
    public class Business
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Category { get; set; }
        public int Currency { get; set; }
        public string CollectionAccount { get; set; }
        public bool ClosedGroupAccess { get; set; }
        public bool ReadThirdParty { get; set; }
        public bool WriteThirdParty { get; set; }
        public int? ThirdPartyMode { get; set; }
        public decimal? FixedCollectionLimit { get; set; }
        public decimal? UpperCollectionLimit { get; set; }
        public decimal? LowerCollectionLimit { get; set; }
        public decimal? TotalCollectionLimit { get; set; }
        public decimal? CollectionsCountLimit { get; set; }
        public bool? PledgesAllowed { get; set; }
        public bool? PrintReceipt { get; set; }
        public string ReceiptNoMode { get; set; }
        public string ReceiptNoPrefix { get; set; }
        public int? ReceiptNoLength { get; set; }
        public string Dateexpires { get; set; }
        public string StatusDesc { get; set; }
        public bool? ShowOnWeb { get; set; }
        public bool? ShowOnApp { get; set; }
        public bool? ShowOnTp { get; set; }
        public bool? IsSearchable { get; set; }
        public bool? HasSubItems { get; set; }
        public int? Status { get; set; }
        public DateTime? DateCreated { get; set; }
        public DateTime? DateUpdated { get; set; }
        public string Businessid { get; internal set; }
    }
}
