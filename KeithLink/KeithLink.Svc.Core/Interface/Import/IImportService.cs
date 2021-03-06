﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using KeithLink.Svc.Core.Models.Lists;
using KeithLink.Svc.Core.Models.Profile;
using KeithLink.Svc.Core.Models.SiteCatalog;

namespace KeithLink.Svc.Core.Interface.Import
{
    public interface IImportService {
        string Errors { get; }

        string Warnings { get; }

        ListModel BuildList(UserProfile user, UserSelectedContext catalogInfo, ListImportFileModel file);

    }
}
