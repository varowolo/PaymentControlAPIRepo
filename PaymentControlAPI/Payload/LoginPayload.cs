using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PaymentControlAPI.Payload
{
    public class LoginPayload
    {
        public string email { get; set; }
        //public string username { get; set; }
        public string password { get; set; }
        public string requestId { get; set; }
        public string signature { get; set; }
    }

}
