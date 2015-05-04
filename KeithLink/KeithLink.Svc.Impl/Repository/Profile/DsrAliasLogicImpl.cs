﻿using KeithLink.Svc.Core.Interface.Cache;
using KeithLink.Svc.Core.Interface.Profile;
using KeithLink.Svc.Core.Models.Profile;
using KeithLink.Svc.Core.Models.Profile.EF;
using KeithLink.Svc.Impl.Repository.EF.Operational;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Impl.Repository.Profile {
    public class DsrAliasLogicImpl : IDsrAliasLogic {
        #region attributes
        protected string CACHE_GROUPNAME { get { return "Profile"; } }
        protected string CACHE_NAME { get { return "Profile"; } }
        protected string CACHE_PREFIX { get { return "Default"; } }

        private readonly ICacheRepository _cache;
        private readonly IDsrAliasRepository _repo;
        private readonly IUnitOfWork _uow;
        #endregion

        #region ctor
        public DsrAliasLogicImpl(IDsrAliasRepository aliasRepo, IUnitOfWork uow, ICacheRepository cacheRepo) {
            _cache = cacheRepo;
            _repo = aliasRepo;
            _uow = uow;
        }
        #endregion

        #region methods
        public DsrAlias CreateDsrAlias(Guid userId, string email, Dsr dsr) {
            DsrAlias alias = new DsrAlias();
            alias.UserId = userId;
            alias.UserName = email;
            alias.BranchId = dsr.Branch;
            alias.DsrNumber = dsr.DsrNumber;

            _repo.Create(alias);
            _uow.SaveChanges();

            _cache.RemoveItem(CACHE_GROUPNAME, CACHE_PREFIX, CACHE_NAME, GetCacheKey(email));

            return alias;
        }

        public void DeleteDsrAlias(int dsrAliasId, string email) {
            _repo.Delete(d => d.Id.Equals(dsrAliasId));
            _uow.SaveChanges();

            _cache.RemoveItem(CACHE_GROUPNAME, CACHE_PREFIX, CACHE_NAME, GetCacheKey(email));
        }

        public List<DsrAlias> GetAllDsrAliasesByUserId(Guid userId) {
            return _repo.ReadByUser(userId).ToList();
        }

        private string GetCacheKey(string email) {
            return string.Format("{0}-{1}", "bek", email);
        }

        #endregion
    }
}
