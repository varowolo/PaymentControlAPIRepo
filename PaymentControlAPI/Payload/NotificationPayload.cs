using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PaymentControlAPI.Payload
{
    public class NotificationPayload
    {
        public string controlId { get; set; }
        public string accountNumber { get; set; }
        public string notificationType { get; set; }
        public string notificationMessage { get; set; }
        public string createDate { get; set; }
        public string requestId { get; set; }
        public string signature { get; set; }

    }

}
