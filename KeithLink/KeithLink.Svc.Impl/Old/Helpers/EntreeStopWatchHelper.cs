using KeithLink.Common.Core.Interfaces.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Impl.Helpers
{
    public static class EntreeStopWatchHelper
    {
        public static Stopwatch GetStopWatch(bool gettiming)
        {
            Stopwatch stopWatch = null;
#if DEBUG
            if (gettiming){
                stopWatch = new Stopwatch(); 
                stopWatch.Start();
            }
#endif
            return stopWatch;
        }

        public static Stopwatch Read(this Stopwatch stopWatch, IEventLogRepository _log, string msg)
        {           
            if(stopWatch != null)
            {
                StackTrace st = new StackTrace();
                stopWatch.Stop();
                _log.WriteInformationLog(string.Format("({0}) {1}: {2}ms",
                                                       st.FrameCount,
                                                       msg,
                                                       stopWatch.ElapsedMilliseconds));
                stopWatch.Restart();
            }
            return stopWatch;
        }
    }
}
