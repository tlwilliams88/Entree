using KeithLink.Common.Core;
using KeithLink.Svc.Core.Interface.Email;
using KeithLink.Svc.Core.Interface.Profile;
using KeithLink.Svc.Core.Interface.Profile.PasswordReset;
using KeithLink.Svc.Core.Models.EF;
using KeithLink.Svc.Impl.Repository.EF.Operational;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KeithLink.Svc.Core.Extensions;
using KeithLink.Svc.Core.Models.Profile;
using KeithLink.Common.Core.Logging;


namespace KeithLink.Svc.Impl.Logic.InternalSvc
{
	public class InternalPasswordResetRequestLogicImpl: IInternalPasswordResetLogic
	{
		private const string resetEmailTemplateKey = "ResetPasswordRequest";
		private const string newUserEmailTemplateKey = "CreatedUserWeclome";

		private readonly IUnitOfWork unitOfWork;
		private readonly IUserProfileLogic userProfileLogic;
		private readonly IPasswordResetRequestRepository passwordResetRequestRepository;
		private readonly IMessageTemplateRepository messageTemplateRepository;
		private readonly IEmailClient emailClient;
		private readonly IEventLogRepository eventLog;


		public InternalPasswordResetRequestLogicImpl(IUnitOfWork unitOfWork, IUserProfileLogic userProfileLogic, IPasswordResetRequestRepository passwordResetRequestRepository,
			IMessageTemplateRepository messageTemplateRepository, IEmailClient emailClient, IEventLogRepository eventLog)
		{
			this.unitOfWork = unitOfWork;
			this.userProfileLogic = userProfileLogic;
			this.passwordResetRequestRepository = passwordResetRequestRepository;
			this.messageTemplateRepository = messageTemplateRepository;
			this.emailClient = emailClient;
			this.eventLog = eventLog;
		}

		public void GenerateNewUserPasswordLink(string emailAddress)
		{
			var profile = userProfileLogic.GetUserProfile(emailAddress);

			if (profile.UserProfiles.Count() == 0 || profile.UserProfiles[0].IsInternalUser) //Profile not found, do nothing. Don't do anything if internal user
				return;

			var token = Crypto.GenerateRandomToken();
			var passwordRequest = new PasswordResetRequest()
			{
				UserId = profile.UserProfiles[0].UserId,
				Token = token,
				Expiration = DateTime.UtcNow.AddYears(5) //Don't want this link to expire for new accounts, but 5 years is plenty of time
			};

			passwordResetRequestRepository.Create(passwordRequest);

			unitOfWork.SaveChanges();

			SendResetPasswordEmail(emailAddress, token, newUserEmailTemplateKey, "setpassword");
		}

		public void GeneratePasswordResetLink(string emailAddress)
		{
			//Lookup the user profile
			var profile = userProfileLogic.GetUserProfile(emailAddress);

			if (profile.UserProfiles.Count() == 0 || profile.UserProfiles[0].IsInternalUser) //Profile not found, do nothing. Don't do anything if internal user
				return;

			var token = Crypto.GenerateRandomToken();
			var passwordRequest = new PasswordResetRequest()
			{
				UserId = profile.UserProfiles[0].UserId,
				Token =token,
				Expiration = DateTime.UtcNow.AddDays(3) //Request is valid for 3 days
			};

			passwordResetRequestRepository.Create(passwordRequest);

			unitOfWork.SaveChanges();

			SendResetPasswordEmail(emailAddress, token, resetEmailTemplateKey, "forgotpassword");

		}
		
		public bool IsTokenValid(string token)
		{
			return passwordResetRequestRepository.Read(p => p.Token.Equals(token) && !p.Processed && p.Expiration > DateTime.UtcNow).Any();
		}

		public bool ResetPassword(ResetPasswordModel resetPassword)
		{
			try
			{
				//Retrieve the reset request, and verify that it's still valid
				var request = passwordResetRequestRepository.Read(p => p.Token.Equals(resetPassword.Token)).FirstOrDefault();

				if (request == null || request.Processed || request.Expiration < DateTime.UtcNow)
					return false; //This is an invalid token

				userProfileLogic.ResetPassword(request.UserId, resetPassword.Password);

				request.Processed = true;
				passwordResetRequestRepository.Update(request);
				unitOfWork.SaveChanges();


				return true;
			}
			catch (Exception ex)
			{
				eventLog.WriteErrorLog("Password reset error", ex);
				return false;
			}
		}

		private void SendResetPasswordEmail(string emailAddress, string token, string templateKey, string baseURL)
		{
			var template = messageTemplateRepository.Read(m => m.TemplateKey.Equals(templateKey)).FirstOrDefault();

			if (template == null)
				throw new Exception("Reset Password Request email template not found");

			emailClient.SendTemplateEmail(template.ToMessageTemplateModel(), new List<string>() { emailAddress }, new { resetLink = string.Format("{0}/#/{1}/?t={2}", Configuration.EntreeSiteURL, baseURL, Uri.EscapeDataString(token)) });

		}



		
	}
}
