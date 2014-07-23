using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using log4net.ObjectRenderer;

namespace KeithLink.Common.Core.Logging.Log4Net
{
    public class Log4NetExceptionRenderer : IObjectRenderer
    {
        #region IObjectRenderer Members

        public void RenderObject(RendererMap rendererMap, object obj, System.IO.TextWriter writer)
        {
            Exception ex = (Exception)obj;

            while (ex != null)
            {
                writer.WriteLine(ex.ToString() + (ex.StackTrace != null ? "\r\n" + ex.StackTrace : string.Empty));
                ex = ex.InnerException;
            }

        }

        #endregion
    }
}
