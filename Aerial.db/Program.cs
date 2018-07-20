using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace Aerial.db
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

			Application.ThreadException += new System.Threading.ThreadExceptionEventHandler(Application_ThreadException);
			AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);

			Application.Run(new MainForm());
        }

		static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e) {
			try {
				WriteErrorMessage(((Exception)e.ExceptionObject).ToString());
			}
			catch { }
		}

		static void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e) {
			try {
				WriteErrorMessage(e.Exception.ToString());
			}
			catch { }
		}

		static void WriteErrorMessage(string MessageText) {
			try {
				System.IO.File.WriteAllText(
					string.Format("{0}\\Aerial.db.{1}.txt", System.IO.Path.GetTempPath(), DateTime.Now.Ticks),
					MessageText);
			}
			catch { }
			
		}


    }
}
