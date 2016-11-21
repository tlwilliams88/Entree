using KeithLink.Svc.Core.Models.Template;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Core.Interface.Templates
{
    public interface ITemplatesRepository
    {
        Stream Get(TemplateRequestModel templateRequest);
    }
}
