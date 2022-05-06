using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using PaymentControlAPI.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace PaymentControlAPI.BLL
{
    public class SendEmail
    {
        public readonly IConfiguration _config;
        public IHostEnvironment _env;
        public SendEmail(IConfiguration config, IHostEnvironment env)
        {
            _config = config;
            _env = env;
        }

        public  bool Send(string subject, string message, string ReceiverEmail, string attachementfilepath = null, string MailsToCopy = "")
        {
            bool flag = false;

            string sender =_config.GetSection("ExchangeUserName").Value.ToString();
            string senderpassword = _config.GetSection("ExchangeUserPassword").Value.ToString();
            string receiver = ReceiverEmail;
            //  WebConfigurationManager.AppSettings("ReceiverEmail").ToString()
            int mailportnumber = int.Parse(_config.GetSection("ExchangePort").Value.ToString());
            string mailserver = _config.GetSection("ExchangeHostIP").Value.ToString();
            string maildisplayName = _config.GetSection("maildisplayName").Value.ToString(); 
            //   Dim mailreceivers As String()
            // string deliverystatus = "0";

            System.Net.Mail.MailMessage mailmsg = new System.Net.Mail.MailMessage();

            System.Net.Mime.ContentType mimeType = new System.Net.Mime.ContentType("text/html");
            string body = System.Web.HttpUtility.HtmlDecode(message);
            mailmsg.SubjectEncoding = Encoding.UTF8;
            mailmsg.From = new System.Net.Mail.MailAddress(sender, maildisplayName, Encoding.UTF8);
            string[] receivers = receiver.Split(',');
            for (int i = 0; i <= receiver.Split(',').Length - 1; i++)
            {
                if (receivers[i].Contains("@"))
                {
                    mailmsg.To.Add(new System.Net.Mail.MailAddress(receivers[i]));
                }
            }
            String[] mailtocopy = MailsToCopy.Split(',');
            if ((MailsToCopy != null))
            {
                if (!MailsToCopy.Contains("nbsp"))
                {
                    for (int i = 0; i <= MailsToCopy.Split(',').Length - 1; i++)
                    {
                        if (mailtocopy[i].Contains("@"))
                        {
                            mailmsg.CC.Add(new System.Net.Mail.MailAddress(mailtocopy[i]));
                        }
                    }

                }
            }
            //Try
            if ((attachementfilepath != null))
            {
                MemoryStream strm = new MemoryStream(File.ReadAllBytes(attachementfilepath));
                System.Net.Mime.ContentType contype = new System.Net.Mime.ContentType();
                contype.MediaType = System.Net.Mime.MediaTypeNames.Application.Octet;
                FileInfo fil = new FileInfo(attachementfilepath);
                contype.Name = fil.Name;
                System.Net.Mail.Attachment atc = new System.Net.Mail.Attachment(strm, contype);
                mailmsg.Attachments.Add(atc);

            }

            try
            {
                System.Net.Mail.SmtpClient client = new System.Net.Mail.SmtpClient(mailserver, mailportnumber);
                client.EnableSsl = true;
                client.UseDefaultCredentials = true;
                client.Credentials = new System.Net.NetworkCredential(sender, senderpassword);
                // client.DeliveryFormat = SmtpDeliveryFormat.SevenBit,
                client.DeliveryMethod = SmtpDeliveryMethod.Network;

                mailmsg.IsBodyHtml = true;
                ////false if the message body contains code
                mailmsg.Priority = System.Net.Mail.MailPriority.High;
                mailmsg.Subject = subject;
                mailmsg.Body = body;
                client.Send(mailmsg);

                mailmsg.DeliveryNotificationOptions = DeliveryNotificationOptions.OnSuccess;
                //Dim i As Integer = mailmsg.DeliveryNotificationOptions
                mailmsg.Attachments.Clear();
                mailmsg.Dispose();
                flag = true;



            }
            catch (Exception ex)
            {
                // Throw New SmtpFailedRecipientException("The following problem occurred when attempting to " + "send your email: " + ex.Message)
                new LogWriter(_env,ex.Message + " " + ex.StackTrace, "ErrorLog");
                return false;



            }
            finally
            {
                mailmsg = null;


            }

            return true;

        }
    }
}
