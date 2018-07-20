using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aerial.db.dal
{
    public class Log
    {
        static public Logger LogWriter = new Logger();
    }

    public class Logger
    {
                public string RootPath = "C:\\";
        public string Applicator = "None";
        public Logger()
        {

        }
        public Logger(string RootPath, string Applicator)
        {
            this.RootPath = RootPath;
            this.Applicator = Applicator;
        }
        
        public void WriteSystem(string Message)
        {
            string FilePath = string.Format("{0}\\System-{1}.log", RootPath, Environment.MachineName);
            WriteToFile(FilePath, Message);
        }

        public void WriteActivity(string Message)
        {
            string FilePath = string.Format("{0}\\Activity-{1}-{2}-{3}.log", RootPath, Environment.MachineName, string.Format("{0}{1}{2}", DateTime.Now.Year, DateTime.Now.Month.ToString().PadLeft(2, '0'), DateTime.Now.Day.ToString().PadLeft(2, '0')), Applicator);
            WriteToFile(FilePath, Message);
        }

        private void WriteToFile(string FilePath, string Message)
        {
            //System.IO.File.AppendAllText(FilePath, string.Format("{0}\r\n{1}\r\n   ---\r\n", DateTime.Now, Message));
        }
    }
}