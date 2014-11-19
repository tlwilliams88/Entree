﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
using KeithLink.Svc.Core.Models.ContentManagement;

namespace KeithLink.Svc.InternalSvc.Interfaces
{
    [ServiceContract]
    interface IContentManagementService
    {
        [OperationContract]
        void CreateContentItem(ContentItemPostModel contentItem);

        [OperationContract]
        List<ContentItemViewModel> ReadActiveContentItemsByBranch(string branchId, int count);

        [OperationContract]
        List<ContentItemViewModel> ReadContentItemsByBranch(string branchId, int count);

        [OperationContract]
        void DeleteContentItemById(int itemId);

        [OperationContract]
        ContentItemViewModel ReadContentItemById(int itemId);
    }
}
