﻿using KeithLink.Svc.Core.Extensions;

using KeithLink.Svc.Core.Interface.Profile;

using KeithLink.Svc.Core.Models.Profile;
using KeithLink.Svc.Core.Models.Profile.EF;

using KeithLink.Svc.Impl.Repository.EF.Operational;

using System;
using System.Collections.Generic;
using System.Linq;

namespace KeithLink.Svc.Impl.Logic.Profile {
    public class DsrAliasLogicImpl : IDsrAliasLogic {
        #region attributes
        private readonly IDsrAliasRepository _repo;
        private readonly IUnitOfWork _uow;
        #endregion

        #region ctor
        public DsrAliasLogicImpl(IDsrAliasRepository aliasRepo, IUnitOfWork uow) {
            _repo = aliasRepo;
            _uow = uow;
        }
        #endregion

        #region methods
        public DsrAliasModel CreateDsrAlias(Guid userId, string email, Dsr dsr) {
            DsrAlias alias = new DsrAlias();
            alias.UserId = userId;
            alias.UserName = email;
            alias.BranchId = dsr.Branch;
            alias.DsrNumber = dsr.DsrNumber;

            _repo.Create(alias);
            _uow.SaveChanges();

            return alias.ToModel();
        }

        public void DeleteDsrAlias(long dsrAliasId, string email) {
            _repo.Delete(d => d.Id.Equals(dsrAliasId));
            _uow.SaveChanges();
        }

        public List<DsrAliasModel> GetAllDsrAliasesByUserId(Guid userId) {
            var dsralias = _repo.ReadByUser(userId).ToList();
            return dsralias.Select(d => d.ToModel()).ToList();
        }
        #endregion
    }
}
