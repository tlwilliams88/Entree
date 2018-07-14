using Dapper;
using Entree.Core.Interface.DataConnection;
using Entree.Core.Interface.UserFeedback;
using Entree.Core.Models.UserFeedback;
using KeithLink.Svc.Impl.Repository.DataConnection;
using System;
using System.Data;

namespace KeithLink.Svc.Impl.Repository.UserFeedback
{
    public class UserFeedbackRepository : IUserFeedbackRepository
    {
        #region attributes
        private readonly IDapperDatabaseConnection _dbConnection;
        #endregion

        #region ctor
        public UserFeedbackRepository(IDapperDatabaseConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }
        #endregion

        #region methods
        /// <summary>
        ///     Create a feedback row for the user
        /// </summary>
        /// <param name="user"></param>
        /// <param name="userFeedback"></param>
        public void SaveUserFeedback(UserFeedbackContext context, Core.Models.UserFeedback.UserFeedback userFeedback)
        {
            if (context == null)
                throw new ArgumentNullException("context");
            if (userFeedback == null)
                throw new ArgumentNullException("userFeedback");

            string sqlCommand = @"
	            INSERT INTO [Messaging].[UserFeedback]
		            (
		            [UserId]
		            ,[UserFirstName]
		            ,[UserLastName]
		            ,[BranchId]	
		            ,[CustomerNumber]
		            ,[CustomerName]
		            ,[SalesRepName]
		            ,[Audience]
		            ,[SourceName]
		            ,[TargetName]
		            ,[SourceEmailAddress]
		            ,[TargetEmailAddress]
		            ,[Subject]
		            ,[Content]
		            ,[BrowserUserAgent]
		            ,[BrowserVendor]
		            )
	            OUTPUT Inserted.Id
	            VALUES
		            (
		            @UserId	
		            ,@UserFirstName
		            ,@UserLastName
		            ,@BranchId
		            ,@CustomerNumber
		            ,@CustomerName
		            ,@SalesRepName
		            ,@Audience
		            ,@SourceName
		            ,@TargetName
		            ,@SourceEmailAddress
		            ,@TargetEmailAddress

		            ,@Subject
		            ,@Content

		            ,@BrowserUserAgent
		            ,@BrowserVendor
		            )

                SELECT SCOPE_IDENTITY()
                ";

            var parameters =
                new
                {
                    UserId = context.UserId,
                    UserFirstName = context.UserFirstName,
                    UserLastName = context.UserLastName,
                    BranchId = context.BranchId,
                    CustomerNumber = context.CustomerNumber,
                    CustomerName = context.CustomerName,
                    SalesRepName = context.SalesRepName,
                    SourceName = context.SourceName,
                    TargetName = context.TargetName,
                    SourceEmailAddress = context.SourceEmailAddress,
                    TargetEmailAddress = context.TargetEmailAddress,

                    Subject = userFeedback.Subject,
                    Content = userFeedback.Content,

                    Audience = userFeedback.Audience.ToString(),
                    BrowserUserAgent = userFeedback.BrowserUserAgent,
                    BrowserVendor = userFeedback.BrowserVendor,
                };

            _dbConnection.Execute(sqlCommand, parameters);

        }

         #endregion
    }
}