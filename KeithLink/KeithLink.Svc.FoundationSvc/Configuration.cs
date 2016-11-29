using KeithLink.Common.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KeithLink.Svc.FoundationSvc
{
        public class Configuration : ConfigurationFacade
        {
            #region attributes
            private const string KEY_PIPELINE_LOGGING_CONNECTIONSTRING = "EnableLoggingPipeline";
            #endregion

            #region properties
            public static bool EnableLoggingPipeline
        {
                get
                {
                    bool retVal;

                    bool.TryParse(GetValue(KEY_PIPELINE_LOGGING_CONNECTIONSTRING, "false"), out retVal);

                    return retVal;
                }
            }

            #endregion
        }
    }