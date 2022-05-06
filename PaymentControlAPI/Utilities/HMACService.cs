//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;

//namespace PaymentControlAPI.Utilities
//{
//    public class HMACService
//    {
//    }
//}


using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
 
namespace PaymentControlAPI.Utilities  //HashingAlgos
{
    public class HMACService
    {
        public string ComputeHmac(string message, HMAC hmac)
        {
            return Convert.ToBase64String(hmac.ComputeHash(Encoding.UTF8.GetBytes(message)));
        }
    }
}