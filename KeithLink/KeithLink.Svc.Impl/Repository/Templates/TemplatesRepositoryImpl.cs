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
            Assembly assembly = Assembly.Load("Keithlink.Svc.Impl");
            if (templateRequest.Name != null)
            {
                if (templateRequest.Name.Equals("importcustominventory", StringComparison.CurrentCultureIgnoreCase) && 
                    templateRequest.Format.Equals("csv", StringComparison.CurrentCultureIgnoreCase))
                {
                    return assembly.GetManifestResourceStream
                        ("KeithLink.Svc.Impl.Templates.importcustominventory.csv");
                }
            }
            return null;
        }
    }
}
