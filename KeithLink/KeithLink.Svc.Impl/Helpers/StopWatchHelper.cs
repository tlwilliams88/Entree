using KeithLink.Common.Core.Interfaces.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Impl.Helpers
{
    public class EntreeStopWatchHelper
    {
        public static System.Diagnostics.Stopwatch GetStopWatch()
        {
            var stopWatch = new System.Diagnostics.Stopwatch(); //Temp: Remove
            stopWatch.Start();
            return stopWatch;
        }

        public static void ReadStopwatch(System.Diagnostics.Stopwatch stopWatch, IEventLogRepository _log, string msg)
        {
            stopWatch.Stop();
            _log.WriteInformationLog(string.Format("{0}: {1}ms", msg, stopWatch.ElapsedMilliseconds));
            stopWatch.Restart();
        }
    }
}
