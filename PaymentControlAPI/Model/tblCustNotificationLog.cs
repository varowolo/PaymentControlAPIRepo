using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PaymentControlAPI.Model
{
    public class tblCustNotificationLog
    {
        public long Id { get; set; }
        public string controlId { get; set; }
        public string accountNumber { get; set; }
        public string notificationType { get; set; }
        public string Subject { get; set; }
        public string notificationMessage { get; set; }
        public string RequestId { get; set; }
        public string Status { get; set; }
        public string Signature { get; set; }
        public DateTime createDate { get; set; }        
        public bool notificationStatus { get; set; }
        public DateTime notificationDatetime { get; set; }
        public string emailDestination { get; set; }
    }
}
