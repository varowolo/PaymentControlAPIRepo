using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace PaymentControlAPI.Utilities
{
    public class LogWriter
    {
        private readonly IHostEnvironment env;
        private string m_exePath = string.Empty;
        public LogWriter(IHostEnvironment env, string logMessage, string folder)
        {
            this.env = env;
            LogWrite(logMessage, folder);
        }

        //public LogWriter(string logMessage)
        //{
        //    LogWrite(logMessage);
        //}
        public string LogWrite(string logMessage, string folder)
        {

            try
            {
                Directory.CreateDirectory(env.ContentRootPath + "/"+folder+"/");
                m_exePath = env.ContentRootPath + "/" + folder + "/" + DateTime.Now.ToString("dd-MM-yyyy") + ".txt";
                if (!File.Exists(m_exePath))
                {
                    using (StreamWriter sw = File.CreateText(m_exePath))
                    {
                        Log(logMessage, sw);
                    }
                }
                else
                {
                    using (StreamWriter w = File.AppendText(m_exePath))
                    {
                        Log(logMessage, w);
                    }
                }
                return null;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public string Log(string logMessage, TextWriter txtWriter)
        {
            try
            {
                txtWriter.Write("\r\nLog Entry : ");
                txtWriter.WriteLine("{0} {1}", DateTime.Now.ToLongTimeString(),
                    DateTime.Now.ToLongDateString());
                txtWriter.WriteLine("  :");
                txtWriter.WriteLine("  :{0}", logMessage);
                txtWriter.WriteLine("-------------------------------");
                return null;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
    }
}
