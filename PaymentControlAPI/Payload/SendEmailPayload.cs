using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PaymentControlAPI.Payload
{
    public class SendEmailPayload
    {
        [JsonProperty("to")]
        public string To { get; set; }
        [JsonProperty("subject")]
        public string Subject { get; set; }
        [JsonProperty("requestId")]
        public string RequestId { get; set; }
        [JsonProperty("body")]
        public string Body { get; set; }
        [JsonProperty("signature")]
        public string Signature { get; set; }
       

    }
}
