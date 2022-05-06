using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PaymentControlAPI.Payload
{
    public class AccountNumberPayload
    {
        public string accountNumber { get; set; }
        public string requestId { get; set; }
        public string signature { get; set; }
    }
}
