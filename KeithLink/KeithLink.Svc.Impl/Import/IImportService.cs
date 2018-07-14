using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Entree.Core.Models.Lists;
using Entree.Core.Models.Profile;
using Entree.Core.Models.SiteCatalog;

namespace Entree.Core.Interface.Import
{
    public interface IImportService {
        string Errors { get; }

        string Warnings { get; }

        ListModel BuildList(UserProfile user, UserSelectedContext catalogInfo, ListImportFileModel file);

    }
}
