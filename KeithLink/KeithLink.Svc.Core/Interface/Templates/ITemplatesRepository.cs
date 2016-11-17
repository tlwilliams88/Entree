using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Core.Interface.Templates
{
    public interface ITemplatesRepository
    {
        Stream Get(string name);
    }
}
