using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aerial.db.dal {
	public class Email {
		public static void SendEmail(string MessageText, string Version) {
			System.Net.Mail.MailMessage message = new System.Net.Mail.MailMessage();
			message.From = new System.Net.Mail.MailAddress("dan@aggtool.com");
			message.To.Add("dan@aggtool.com");
			message.Subject = string.Format("Error message from Aerial.db - WOP");
			message.Body = string.Format("Version: {0}\r\nMachine: {1}\r\nUser: {2}\r\nMessage: {3}",
				Version,
				System.Environment.MachineName,
				System.Environment.UserName,
				MessageText);

			System.Net.Mail.SmtpClient client = new System.Net.Mail.SmtpClient("smtp.gmail.com");
			client.Port = 587;
			client.UseDefaultCredentials = false;
			client.Credentials = new System.Net.NetworkCredential("dan@aggtool.com", "MjkDUBmouxNztGmm8aH-");
			client.EnableSsl = true;

			try {
				client.SendAsync(message, null);
			}
			catch { }
		}
	}
}
