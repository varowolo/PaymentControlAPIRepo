using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PaymentControlAPI.Models
{
    public class LoginUserPayload
    {
        public string username { get; set; }
        public string password { get; set; }
    }
}
