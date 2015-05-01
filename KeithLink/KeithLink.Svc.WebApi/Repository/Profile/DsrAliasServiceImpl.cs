﻿using KeithLink.Svc.Core.Interface.Profile;
using KeithLink.Svc.Core.Models.Profile;
using KeithLink.Svc.Core.Models.Profile.EF;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KeithLink.Svc.WebApi.Repository.Profile {
    public class DsrAliasServiceImpl : IDsrAliasService {
        #region attributes
		private com.benekeith.ProfileService.IProfileService _client;
		#endregion

		#region ctor
        public DsrAliasServiceImpl(com.benekeith.ProfileService.IProfileService client)
		{
			_client = client;
		}
		#endregion

        #region methods
        public void CreateDsrAlias(Guid userId, string email, Dsr dsr) {
            _client.CreateDsrAlias(userId, email, dsr);
        }

        public void DeleteDsrAlias(int dsrAliasId) {
            _client.DeleteDsrAlias(dsrAliasId);
        }

        public List<DsrAlias> GetAllDsrAliasesByUserId(Guid userId) {
            return _client.GetAllDsrAliasesByUserId(userId).ToList();
        }
        #endregion
    }
}