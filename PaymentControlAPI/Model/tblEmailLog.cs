using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PaymentControlAPI.Model
{
    public class tblEmailLog
    {

        public long Id { get; set; }
        public string ToEmail { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public string RequestId { get; set; }
        public string Status { get; set; }
        public string Signature { get; set; }
        public DateTime DropTimestamp { get; set; }
        public bool emailStatus { get; set; }
        public DateTime emailDatetime { get; set; }

    }
}
