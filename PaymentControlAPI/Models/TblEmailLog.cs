using System;
using System.Collections.Generic;

namespace PaymentControlAPI.Models
{
    public partial class TblEmailLog
    {
        public long Id { get; set; }
        public string ToEmail { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public string RequestId { get; set; }
        public string Status { get; set; }
        public string Signature { get; set; }
        public DateTime DropTimestamp { get; set; }
        public bool? EmailStatus { get; set; }
        public DateTime? EmailDatetime { get; set; }
        public string acknowledgedResponse { get; set; }
        public string acknowledgedReason { get; set; }
        
    }
}
