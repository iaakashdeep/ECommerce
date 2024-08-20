using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Utility.Logging
{
    public sealed class LoggerSingleton
    {
        
        private LoggerSingleton() { }
        private static readonly Lazy<LoggerSingleton> Instance = new Lazy<LoggerSingleton>();

        public static LoggerSingleton getInstance
        {
            get
            {
                return Instance.Value;
            }
        }

        public void Log(string message)
        {
            string fileName = string.Format("{0}_{1}.log", "Exception", DateTime.Now.ToShortDateString());
            string logFilePath = string.Format(@"{0\{1}}", AppDomain.CurrentDomain.BaseDirectory, fileName);
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("=====================");
            sb.AppendLine(DateTime.Now.ToString());
            sb.AppendLine(message);
            using (StreamWriter sw = new StreamWriter(logFilePath, true))
            {
                sw.Write(sb.ToString());
                sw.Flush();
            }
        }
    }
}
