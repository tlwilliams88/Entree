using Dapper;
using KeithLink.Svc.Core.Enumerations.Messaging;
using KeithLink.Svc.Core.Interface.UserFeedback;
using KeithLink.Svc.Core.Models.Profile;
using KeithLink.Svc.Core.Models.SiteCatalog;
using KeithLink.Svc.Impl.Repository.DataConnection;
using System;
using System.Data;

namespace KeithLink.Svc.Impl.Repository.UserFeedback
{
    public class UserFeedbackRepository : DapperDatabaseConnection, IUserFeedbackRepository
    {
        #region attributes
        #endregion

        #region ctor
        public UserFeedbackRepository() : base(Configuration.BEKDBConnectionString)
        {
        }
        #endregion

        #region methods
        /// <summary>
        ///     Create a feedback row for the user
        /// </summary>
        /// <param name="user"></param>
        /// <param name="userFeedback"></param>
        public int SaveUserFeedback(UserFeedbackContext context, Core.Models.UserFeedback.UserFeedback userFeedback)
        {
            if (context == null)
                throw new ArgumentNullException("context");
            if (userFeedback == null)
                throw new ArgumentNullException("userFeedback");

            var command = new CommandDefinition(
                "Messaging.AddUserFeedback",
                new
                {
                    UserId = context.UserId,
                    BranchId = context.BranchId,
                    CustomerName = context.CustomerName,
                    SalesRepName = context.SalesRepName,
                    SourceName = context.SourceName,
                    TargetName = context.TargetName,
                    SourceEmailAddress = context.SourceEmailAddress,
                    TargetEmailAddress = context.TargetEmailAddress,
                    Subject = userFeedback.Subject,
                    Content = userFeedback.Content,
                },
                commandType: CommandType.StoredProcedure
            );

            int Id = ExecuteScalarCommand<int>(command);

            return Id;
        }

         #endregion
    }
}