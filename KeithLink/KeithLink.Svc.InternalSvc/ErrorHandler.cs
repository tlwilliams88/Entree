﻿using Autofac;
using Autofac.Integration.Wcf;
using KeithLink.Common.Core.Interfaces.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Dispatcher;
using System.Web;

namespace KeithLink.Svc.InternalSvc
{
    public class ErrorHandler : IErrorHandler {
        private IEventLogRepository eventLogRepository;

        public bool HandleError(Exception error) {
            eventLogRepository = ((IContainer) AutofacHostFactory.Container).Resolve<IEventLogRepository>();

            eventLogRepository.WriteErrorLog("Unhandled Service Exception", error);

            KeithLink.Common.Impl.Email.ExceptionEmail.Send(error);

            return false;
        }

        public void ProvideFault(Exception error, System.ServiceModel.Channels.MessageVersion version, ref System.ServiceModel.Channels.Message fault) {
            //Do Nothing
        }
    }
}