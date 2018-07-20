using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace Aerial.db.WOP
{
    public static class Program
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
			try {
				Application.Run(new Form1());
			}
			catch (Exception ex) {
				MessageBox.Show(ex.ToString());
			}
        }

		static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e) {
			try {
				Aerial.db.dal.Email.SendEmail(((Exception)e.ExceptionObject).ToString(), Aerial.db.WOP.Form1.VERSION);
				MessageBox.Show("An error has occurred and has been logged.");
			}
			catch { }
		}

		static void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e) {
			try {
				Aerial.db.dal.Email.SendEmail(e.Exception.ToString(), Aerial.db.WOP.Form1.VERSION);
				MessageBox.Show("An error has occurred and has been logged.");
			}
			catch { }
		}
    }
}
