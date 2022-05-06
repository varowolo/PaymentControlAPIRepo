using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PaymentControlAPI.BLL;
using PaymentControlAPI.Model;
using PaymentControlAPI.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PaymentControlAPI.Utilities
{
    internal class SendLoggedMail : IHostedService, IDisposable
    {

        private Timer _timer;
        private readonly ILogger _logger;
        private readonly IConfiguration _config;
        private readonly IWebHostEnvironment _hosting;
        public static DateTime haverun2 = DateTime.Now.AddDays(-1);
        public IServiceProvider _serviceProvider;

        public SendLoggedMail(ILogger<SendLoggedMail> logger, IConfiguration config, IWebHostEnvironment hosting, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _config = config;
            _hosting = hosting;
            _serviceProvider = serviceProvider;
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Timed Background Service is starting.");
            Thread t1 = new Thread(new ThreadStart(MainWorker));
            t1.Start();
            //_timer = new Timer(DoWork, null, TimeSpan.Zero,
            // TimeSpan.FromSeconds(300))
            return Task.CompletedTask;

        }


        private void MainWorker()
        {
            _logger.LogInformation("Background Service is working.");
            while (true)
            {
                DoWork(null);
                Thread.Sleep(30000);
            }
        }

        private void DoWork(object state)
        {
            var db = _serviceProvider.CreateScope().ServiceProvider.GetRequiredService<PaymentControlDBContext>();
            _logger.LogInformation("Background Service is working.");
            var pendingEmail = db.TblEmailLog.Where(o => o.Status == "Pending").ToList();
            if (pendingEmail.Count() > 0)
            {
                foreach (var item in pendingEmail)
                {
                    using (StreamReader reader = new StreamReader(_hosting.WebRootPath + "C:/Users/Arowolo/Documents/PaymentControlAPI_New/PaymentControlAPI/htmlDemopage.html"))
                    {
                        string mainbody = string.Empty;
                        mainbody = reader.ReadToEnd();
                        // mainbody = mainbody.Replace("**firstname**", item.EmailBody.Split('~') [(0)].Replace("**message**", item.Body.Split('~')[1]).Replace("**subject**", item.Subject);

                        var a = new SendEmail(_config, _hosting).Send(item.Subject, item.Body, item.ToEmail);

                        if (a)
                        {
                            item.Status = "Sent";
                            item.EmailDatetime = DateTime.Now;
                            db.TblEmailLog.Attach(item);
                            db.Entry(item).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                        }

                    }
                }
                db.SaveChanges();
            }
        }



        public Task StopAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}


//oniyideoluwatobiloba747 @gmail.com