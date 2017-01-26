﻿using KeithLink.Svc.Core.Models.Marketing;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Core.Interface.Marketing
{
    public interface ICatalogCampaignLogic
    {
        CatalogCampaignReturnModel GetCampaign(int id);
        CatalogCampaignReturnModel GetCampaignByUri(string uri);
    }
}
