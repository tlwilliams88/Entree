using System;
using System.Net.Mail;
using System.Text;

namespace KeithLink.Common.Core.Email {
    public static class ExceptionEmail {
        #region attributes
        private const string EMAIL_FAILURE_SUBJECT = "Exception encountered in KeithLink Order Service";
        #endregion

        public static void Send(Exception currentExcpetion, string additionalMessage = null) {
            using (MailMessage msg = new MailMessage()) {
                msg.To.Add(Configuration.FailureEmailAddress);
                msg.From = new MailAddress(Configuration.FromEmailAddress);
                msg.Subject = EMAIL_FAILURE_SUBJECT;
                msg.Priority = MailPriority.High;

                StringBuilder body = new StringBuilder();
                body.Append("An exception has occurred in the KeithLink Order Service. Please contact the on-call person within the E0Business group and ");
                body.AppendLine("open an incident.");

                if (additionalMessage != null) { body.Append(additionalMessage); }

                body.AppendLine(GetExceptionDetails(currentExcpetion));

                if (Configuration.IsProduction) {
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
