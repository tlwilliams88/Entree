﻿using CommerceServer.Foundation;
using KeithLink.Common.Core.Interfaces.Logging;
using Entree.Core.Interface.Profile;
using Entree.Core.Models.Profile;
using System;
using System.Collections.Generic;
using System.Linq;
using Entree.Core.Models.Generated;
using KeithLink.Common.Core.Extensions;

namespace KeithLink.Svc.Impl.Repository.Profile
{
    public class BaseOrgRepository
    {
        #region methods


        public void AddUserToOrg(Guid orgId, Guid userId)
        {
            CommerceCreate<UserOrganizations> createOrg = new CommerceCreate<UserOrganizations>("UserOrganizations");
            createOrg.Model.Id = Guid.NewGuid().ToCommerceServerFormat();
            createOrg.Model.OrganizationId = orgId.ToCommerceServerFormat();
            createOrg.Model.UserId = userId.ToCommerceServerFormat();
            createOrg.CreateOptions.ReturnModel = new UserOrganizations();

            createOrg.Model.UserOrganizationKey = GetUserOrgKey(orgId, userId);
            CommerceResponse res = Svc.Impl.Helpers.FoundationService.ExecuteRequest(createOrg.ToRequest());
        }

        protected static string GetUserOrgKey(Guid orgId, Guid userId)
        {
            return orgId.ToCommerceServerFormat() + "__" + userId.ToCommerceServerFormat();
        }

        public void RemoveUserFromOrg(Guid orgId, Guid userId)
        {
            var deleteUser = new CommerceDelete<Svc.Core.Models.Generated.UserOrganizations>();
            deleteUser.SearchCriteria.Model.UserOrganizationKey = GetUserOrgKey(orgId, userId);
            deleteUser.DeleteOptions.ReturnDeletedCount = true;
            CommerceResponse response =Svc.Impl.Helpers.FoundationService.ExecuteRequest(deleteUser.ToRequest());

            CommerceDeleteOperationResponse deleteResponse = response.OperationResponses[0]
                as CommerceDeleteOperationResponse;
        }

        protected string GetCacheKey(string setName)
        {
            return "CustomerCache_" + setName;
        }

        protected Entree.Core.Models.Generated.SiteTerm GetOrganizationTypes()
        {
            var siteTermQuery = new CommerceServer.Foundation.CommerceQuery<Entree.Core.Models.Generated.SiteTerm>("SiteTerm");
            siteTermQuery.SearchCriteria.Model.Properties["Id"] = "OrganizationType";
            siteTermQuery.RelatedOperations.Add(
                new CommerceQueryRelatedItem<Entree.Core.Models.Generated.SiteTermElement>
                    (Entree.Core.Models.Generated.SiteTerm.RelationshipName.Elements));
            CommerceQueryOperationResponse res = (Svc.Impl.Helpers.FoundationService.ExecuteRequest(siteTermQuery.ToRequest())).OperationResponses[0] as CommerceQueryOperationResponse;

            return new Entree.Core.Models.Generated.SiteTerm(res.CommerceEntities[0]);
        }
        #endregion
    }
}
