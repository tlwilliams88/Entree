using KeithLink.Svc.Core.Interface.Templates;
using KeithLink.Svc.Core.Models.Template;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Impl.Repository.Templates
{
    public class TemplatesRepositoryImpl : ITemplatesRepository
    {
        public Stream Get(TemplateRequestModel templateRequest)
        {
            Stream strm = null;
            if(templateRequest != null && templateRequest.Name != null && templateRequest.Format != null)
            {
                Assembly assembly = Assembly.Load("Keithlink.Svc.Impl");
                if (templateRequest.Name != null)
                {
                    if (templateRequest.Name.Equals("importcustominventory", StringComparison.CurrentCultureIgnoreCase) &&
                        templateRequest.Format.Equals("csv", StringComparison.CurrentCultureIgnoreCase))
                    {
                        strm = assembly.GetManifestResourceStream
                                 ("KeithLink.Svc.Impl.Templates.importcustominventory.csv");
                    }
                }
            }
            else
            {
                throw new ApplicationException("template request needs to include name and format");
            }
            return strm;
        }
    }
}
