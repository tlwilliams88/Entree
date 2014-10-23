using KeithLink.Common.Core.Logging;
using KeithLink.Svc.Core;
using KeithLink.Svc.Core.Interface.Common;
using KeithLink.Svc.Core.Interface.Confirmations;
using KeithLink.Svc.Core.Models.Common;
using KeithLink.Svc.Core.Models.Confirmations;
using KeithLink.Svc.Core.Events.EventArgs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml.Serialization;
using KeithLink.Svc.Core.Extensions;

namespace KeithLink.Svc.Impl.Logic.Confirmations
{
    public class ConfirmationLogicImpl : IConfirmationLogic
    {
        #region attributes
        private IEventLogRepository _log;
        private ISocketListenerRepository _socket;

        private IQueueRepository _confirmationQueue;
        #endregion

        #region constructor
        public ConfirmationLogicImpl(IEventLogRepository eventLogRepository, ISocketListenerRepository socketListenerRepository, IQueueRepository confirmationQueue)
        {
            _log = eventLogRepository;
            _socket = socketListenerRepository;
            _confirmationQueue = confirmationQueue;

            _socket.FileReceived            += SocketFileReceived;
            _socket.ClosedPort              += SocketPortClosed;
            _socket.OpeningPort             += SocketOpeningPort;
            _socket.WaitingConnection       += SocketWaitingConnection;
            _socket.BeginningFileReceipt    += SocketBeginningFileReceipt;
            _socket.ErrorEncountered        += SocketExceptionEncountered;

        }
        #endregion

        #region methods/functions

        /// <summary>
        /// Begin listening for new confirmations
        /// </summary>
        public void Listen()
        {
            _socket.Listen();
        }

        /// <summary>
        /// Deserialize the confirmation
        /// </summary>
        /// <param name="rawConfirmation"></param>
        /// <returns></returns>
        private ConfirmationFile DeserializeConfirmation(string rawConfirmation)
        {
            ConfirmationFile confirmation = new ConfirmationFile();

            StringReader reader = new StringReader(rawConfirmation);
            XmlSerializer xs = new XmlSerializer(confirmation.GetType());

            return (ConfirmationFile) xs.Deserialize(reader);
        }

        /// <summary>
        /// Send serialized file to RabbitMQ, send object to commerce server
        /// </summary>
        /// <param name="file"></param>
        public void ProcessFileData(string[] file)
        {
            try {
                ConfirmationFile confirmation = ParseFile( file );
                PublishToQueue( confirmation, ConfirmationQueueLocation.Default );
            } catch (Exception e) {
                throw e;
            }
        }

        /// <summary>
        /// Publish confirmation file to queue
        /// </summary>
        /// <param name="file"></param>
        /// <param name="location"></param>
        public void PublishToQueue( ConfirmationFile file, ConfirmationQueueLocation location ) {
            string serializedConfirmation = SerializeConfirmation( file );

            _confirmationQueue.SetQueuePath( (int) location );
            _confirmationQueue.PublishToQueue( serializedConfirmation );
        }

        /// <summary>
        /// Get the current top Confirmation from the queue
        /// </summary>
        /// <returns></returns>
        public ConfirmationFile GetFileFromQueue() {
            string fileFromQueue = _confirmationQueue.ConsumeFromQueue();
            if (fileFromQueue == null)
                return null; // a null return indicates no message on queue
            else if (String.IsNullOrEmpty(fileFromQueue))
                throw new ApplicationException("Empty file from Confirmation Queue");

            return DeserializeConfirmation( fileFromQueue );
        }

        /// <summary>
        /// Parse an array of strings as a file
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private ConfirmationFile ParseFile(string[] data)
        {
            ConfirmationFile confirmation = new ConfirmationFile();

            confirmation.Header.Parse(data[0]);

            // Start loop at detail, skip header
            for (int i = 1; i <= data.Length - 1; i++)
            {
                if (data[i].Contains("END###") == false)
                {
                    ConfirmationDetail theDeets = new ConfirmationDetail();
                    theDeets.Parse(data[i]);

                    confirmation.Detail.Add(theDeets);
                }
            }

            return confirmation;
        }

        /// <summary>
        /// Serialize the confirmation
        /// </summary>
        /// <param name="confirmation"></param>
        /// <returns></returns>
        private string SerializeConfirmation(ConfirmationFile confirmation)
        {
            StringWriter writer = new StringWriter();
            XmlSerializer xs = new XmlSerializer(confirmation.GetType());

            xs.Serialize(writer, confirmation);

            return writer.ToString();
        }

        #endregion

        #region events

        public void SocketFileReceived(object sender, ReceivedFileEventArgs e)
        {
            string[] lines = e.FileData.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
            ProcessFileData(lines);
        }

        public void SocketPortClosed(object sender, EventArgs e)
        {
            _log.WriteInformationLog("Confirmation listener port closed");
        }

        public void SocketOpeningPort(object sender, EventArgs e)
        {
            _log.WriteInformationLog("Confirmation listener port opening");
        }

        public void SocketWaitingConnection(object sender, EventArgs e)
        {
            _log.WriteInformationLog("Confirmation listener port connecting");
        }

        public void SocketBeginningFileReceipt(object sender, EventArgs e)
        {
            _log.WriteInformationLog("Confirmation listener beginning file receipt");
        }

        public void SocketExceptionEncountered(object sender, ExceptionEventArgs e)
        {
            _log.WriteErrorLog(e.Exception.Message);
        }

        #endregion

        #region properties
        #endregion
    }
}
