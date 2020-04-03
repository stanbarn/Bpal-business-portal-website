using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace BPal.Business.Portal.Core.Models
{
    public class Collection
    {
        public long Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public long Category { get; set; }

        public long Currency { get; set; }

        public string CollectionAccount { get; set; }

        public bool ClosedGroupAccess { get; set; }

        public bool ReadThirdParty { get; set; }

        public bool WriteThirdParty { get; set; }

        public long ThirdPartyMode { get; set; }

        public long FixedCollectionLimit { get; set; }

        public long UpperCollectionLimit { get; set; }

        public long LowerCollectionLimit { get; set; }

        public long TotalCollectionLimit { get; set; }

        public long CollectionsCountLimit { get; set; }

        public bool PledgesAllowed { get; set; }

        public bool PrintReceipt { get; set; }

        public string ReceiptNoMode { get; set; }

        public string ReceiptNoPrefix { get; set; }

        public long ReceiptNoLength { get; set; }

        public string Dateexpires { get; set; }

        public string StatusDesc { get; set; }

        public bool ShowOnWeb { get; set; }

        public bool ShowOnApp { get; set; }

        public bool ShowOnTp { get; set; }

        public bool IsSearchable { get; set; }

        public object HasSubItems { get; set; }

        public long Status { get; set; }

        public DateTime DateCreated { get; set; }

        public DateTime DateUpdated { get; set; }

        public string Businessid { get; set; }
    }

    public partial class CollectionListResponse
    {
        public List<Collection> Forms { get; set; }

        public long TotalCount { get; set; }

        public long PageSize { get; set; }

        public long CurrentPage { get; set; }

        public long TotalPages { get; set; }

        public bool PreviousPage { get; set; }

        public bool NextPage { get; set; }
    }
}
