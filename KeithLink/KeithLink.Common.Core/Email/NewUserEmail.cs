using System;
using System.Net.Mail;
using System.Text;

namespace KeithLink.Common.Core.Email {
    public static class NewUserEmail {
        #region attributes
        private const string EMAIL_WELCOME_SUBJECT = "Welcome to Entree - Ben E. Keith's Premier ordering system!";
        #endregion

        public static void Send(string emailToAddress, string message) {
            using (MailMessage msg = new MailMessage()) {
                msg.To.Add(emailToAddress);
                msg.CC.Add("gsalazar@credera.com"); msg.CC.Add("chendon@credera.com"); msg.CC.Add("rhedges@credera.com");
                msg.From = new MailAddress(Configuration.FromEmailAddress);
                msg.Subject = EMAIL_WELCOME_SUBJECT;
                msg.Priority = MailPriority.Normal;

                StringBuilder body = new StringBuilder();
                //body.AppendLine("An exception has occurred in the KeithLink Order Service.");
                //body.AppendLine();

                if (message != null) { body.Append(message); }

                if (!Configuration.IsProduction) {
                    body.AppendLine();
                    body.AppendLine();
                    body.AppendLine("****************************************************************************************");
                    body.AppendLine("                          Message Sent From TEST ENVIRONMENT");
                    body.AppendLine("****************************************************************************************");
                }

                msg.Body = body.ToString();

                SmtpClient mailServer = new SmtpClient(Configuration.SmtpServerAddress);
                mailServer.Send(msg);
            }
        }

        private static string GetExceptionDetails(Exception currentException) {
            StringBuilder details = new StringBuilder();

            details.AppendLine();
            details.AppendLine();
            details.AppendLine("Exception details: ");

            details.AppendLine("*****************************************");
            details.AppendLine("               Outer Exception");
            details.AppendLine("*****************************************");
            details.AppendLine(string.Concat("Message: ", currentException.Message));
            details.AppendLine(string.Concat("Source: ", currentException.Source));
            details.AppendLine("Stack: ");
            details.AppendLine(currentException.StackTrace);
            details.AppendLine();

            Exception innerException = currentException.InnerException;

            while (innerException != null) {
                details.AppendLine("*****************************************");
                details.AppendLine("               Inner Exception");
                details.AppendLine("*****************************************");
                details.AppendLine(string.Concat("Message: ", innerException.Message));
                details.AppendLine(string.Concat("Source: ", innerException.Source));
                details.AppendLine("Stack: ");
                details.AppendLine(innerException.StackTrace);
                details.AppendLine();

                innerException = currentException.InnerException;
            }

            return details.ToString();
        }
    }
}
