using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace PaymentControlAPI.Responses
{
    public class GetCustomerInfoResponse
    {
        public string accountName { get; set; }
        public string currencyCode { get; set; }
        public string coreBankingId { get; set; }
        public List<card> cards { get; set; }
    }

    public class card
    {

        //[JsonIgnore]
        public string pan { get; set; }
        public string tokenizedPan { get; set; }
        public string expiry { get; set; }
    }


    public class customerInfo
    {
        public string accountName { get; set; }
        public string currencyCode { get; set; }
        public string coreBankingId { get; set; }
    }


    public class AccountInfo
    {
        public string accountName { get; set; }
        public string currencyCode { get; set; }
        public string coreBankingId { get; set; }
    }

}
