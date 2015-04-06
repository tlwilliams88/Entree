using KeithLink.Svc.Core.Interface.OnlinePayments;
using KeithLink.Svc.Core.Interface.OnlinePayments.Log;
using System;
using System.Data.SqlClient;

namespace KeithLink.Svc.Impl.Repository.OnlinePayments.Log {
    public class KPayLogRepositoryImpl : IKPayLogRepository {
        #region attribute
        private IKPayDBContext _dbContext;
        #endregion

        #region ctor
        public KPayLogRepositoryImpl(IKPayDBContext kpayDbContext) {
            _dbContext = kpayDbContext;
        }
        #endregion

        #region methods
        public void Write(string userName, string message) {
            SqlParameter userNameParm = new SqlParameter("@User", userName);
            SqlParameter messageParm = new SqlParameter("@Msg", message);

            _dbContext.Context.Database.ExecuteSqlCommand(
                    "procInsertApplicationLog @User, @Msg",
                    userNameParm,
                    messageParm
                );
        }
        #endregion
    }
}
