using System;
using System.Collections.Generic;
using System.Text;

namespace BPal.Business.Portal.Core.Models
{
    public class Payout
    {
        public long Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public long Currency { get; set; }

        public bool ReadThirdParty { get; set; }

        public bool WriteThirdParty { get; set; }

        public long ThirdPartyMode { get; set; }

        public string DebitAccount { get; set; }

        public bool IsRoutine { get; set; }

        public long RoutineMode { get; set; }

        public string RoutineSchedules { get; set; }

        public long RoutineCount { get; set; }

        public bool ApprovalRequired { get; set; }

        public bool SetReceivers { get; set; }

        public string CreatedBy { get; set; }

        public object UpdatedBy { get; set; }

        public long Status { get; set; }

        public string StatusDesc { get; set; }

        public object AllowApiAccess { get; set; }

        public string Businessid { get; set; }

        public DateTimeOffset DateUpdated { get; set; }

        public DateTimeOffset DateCreated { get; set; }
    }

    public class PayoutListResponse
    {
        public List<Payout> Forms { get; set; }

        public long TotalCount { get; set; }

        public long PageSize { get; set; }

        public long CurrentPage { get; set; }

        public long TotalPages { get; set; }

        public bool PreviousPage { get; set; }

        public bool NextPage { get; set; }
    }
}
