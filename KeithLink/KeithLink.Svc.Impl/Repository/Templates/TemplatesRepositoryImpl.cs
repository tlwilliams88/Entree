using KeithLink.Svc.Core.Interface.Templates;
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
        public Stream Get(string name)
        {
            Assembly assembly = Assembly.Load("Keithlink.Svc.Impl");
            if (name != null)
            {
                if (name.Equals("importcustominventory", StringComparison.CurrentCultureIgnoreCase))
                {
                    return assembly.GetManifestResourceStream
                        ("KeithLink.Svc.Impl.Templates.importcustominventory.csv");
                }
            }
            return null;
        }
    }
}
