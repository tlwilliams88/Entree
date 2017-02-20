﻿using System;
using System.Net.Mail;
using System.Text;

namespace KeithLink.Common.Impl.Email {
    public static class ExceptionEmail {
        #region attributes
        private const string EMAIL_FAILURE_SUBJECT = "Exception encountered in ";
        private static DateTime delayFilterRabbitMQ;
        #endregion

        public static void Send(Exception currentExcpetion, 
                                string additionalMessage = null, 
                                string subject = null, 
                                Attachment attach = null) {
            using (MailMessage msg = new MailMessage()) {
                msg.To.Add(Configuration.FailureEmailAddress);
                msg.From = new MailAddress(Configuration.FromEmailAddress);
				msg.Subject = string.Format("{0} Exception: {1}", 
                                            ConfigurationHelper.GetActiveConfiguration(), 
                                            string.IsNullOrEmpty(subject) ? 
                                                EMAIL_FAILURE_SUBJECT + Configuration.ApplicationName : 
                                                subject);
                msg.Priority = MailPriority.High;

				if (attach != null)
					msg.Attachments.Add(attach);

                StringBuilder body = new StringBuilder();

				body.AppendLine(string.Format("And exception occured on {0}", System.Environment.MachineName));
				body.AppendLine();
				body.AppendLine();

                //body.AppendLine("An exception has occurred in the KeithLink Order Service.");
                //body.AppendLine();

                if (additionalMessage != null) { body.Append(additionalMessage); }

                body.AppendLine(GetExceptionDetails(currentExcpetion));

                if (!Configuration.IsProduction) {
                    body.AppendLine();
                    body.AppendLine();
                    body.AppendLine("****************************************************************************************");
                    body.AppendLine("                          Message Sent From TEST ENVIRONMENT");
                    body.AppendLine("****************************************************************************************");
                }

                msg.Body = body.ToString();

                SmtpClient mailServer = new SmtpClient(Configuration.SmtpServerAddress);

                if (msg.Body.Contains("Problems encountered while connecting to Rabbit MQ Server."))
                // filter the specific queueconnectionexception down to 1 per 5 minutes
                {
                    if (delayFilterRabbitMQ != null && DateTime.Now - delayFilterRabbitMQ < TimeSpan.FromMinutes(5)) return;
                    delayFilterRabbitMQ = DateTime.Now;
                }

                try
                {
                    mailServer.Send(msg);
                }
                catch// (Exception ex)
                {
                   //TODO:  add event log error message
                }
            }
        }

        private static string GetExceptionDetails(Exception currentException) {
			if (currentException == null)
				return string.Empty;

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

                innerException = innerException.InnerException;
            }

            return details.ToString();
        }
    }
}
