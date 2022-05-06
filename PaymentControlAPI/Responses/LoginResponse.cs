using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PaymentControlAPI.Responses
{
    public class LoginResponse
    {
        public string message { get; set; }
        public string name { get; set; }
        public bool authenticated { get; set; }
    }
}
