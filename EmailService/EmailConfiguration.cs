using System;
using System.Collections.Generic;
using System.Text;

namespace EmailService
{

    //\\emial configuration services to configure sending email from our application
   public class EmailConfiguration
    {
        public string From { get; set; }

        public string StmpServer { get; set; }

        public int port { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }
    }
}
