using System;
using System.Collections.Generic;

namespace PaymentControlAPI.Models
{
    public partial class TblCustNotificationLog
    {
        public long Id { get; set; }
        public string ControlId { get; set; }
        public string AccountNumber { get; set; }
        public string NotificationType { get; set; }
        public string Subject { get; set; }
        public string NotificationMessage { get; set; }
        public string RequestId { get; set; }
        public string Status { get; set; }
        public string Signature { get; set; }
        public string CreateDate { get; set; }
        public bool? NotificationStatus { get; set; }
        public DateTime? NotificationDatetime { get; set; }
        public string EmailDestination { get; set; }
        public string acknowledgedResponse { get; set; }
        public string acknowledgedReason { get; set; }
    }
}
