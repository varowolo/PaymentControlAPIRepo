using System;
using System.Collections.Generic;

namespace PaymentControlAPI.Models
{
    public partial class TblRequestAndReponse
    {
        public long Id { get; set; }
        public string RequestType { get; set; }
        public string RequestPayload { get; set; }
        public DateTime RequestTimestamp { get; set; }
        public string Response { get; set; }
        public DateTime ResponseTimestamp { get; set; }
    }
}
