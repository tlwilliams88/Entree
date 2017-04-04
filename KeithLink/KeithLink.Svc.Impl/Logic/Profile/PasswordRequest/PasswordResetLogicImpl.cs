using KeithLink.Common.Core;
using KeithLink.Common.Core.Interfaces.Logging;

using KeithLink.Svc.Core.Extensions;

using KeithLink.Svc.Core.Interface.Email;
using KeithLink.Svc.Core.Interface.Profile;
using KeithLink.Svc.Core.Interface.Profile.PasswordReset;

using KeithLink.Svc.Core.Models.EF;
using KeithLink.Svc.Core.Models.Profile;

using KeithLink.Svc.Impl.Helpers;

using KeithLink.Svc.Impl.Repository.EF.Operational;

using System;
using System.Collections.Generic;
using System.Linq;

namespace KeithLink.Svc.Impl.Logic.Profile.PasswordRequest {
    public class PasswordResetLogicImpl : IPasswordResetLogic {
        #region attributes
        private const string resetEmailTemplateKey = "ResetPasswordRequest";
        private const string newUserEmailTemplateKey = "CreatedUserWelcome";

        private readonly ICustomerDomainRepository _adRepo;
        private readonly IEmailClient _emailClient;
        private readonly IMessageTemplateRepository _emailTemplateRepo;
        private readonly IEventLogRepository _log;
        private readonly IPasswordResetRequestRepository _passwordRepo;
        private readonly IUserProfileRepository _profileRepo;
        private readonly IUnitOfWork _uow;
        #endregion

        #region ctor
        public PasswordResetLogicImpl(IUnitOfWork unitOfWork, IUserProfileRepository userProfileRepository, ICustomerDomainRepository customerDomainRepository,
                                      IPasswordResetRequestRepository passwordResetRequestRepository, IMessageTemplateRepository messageTemplateRepository, IEmailClient emailClient, 
                                      IEventLogRepository eventLog) {
            _adRepo = customerDomainRepository;
            _emailClient = emailClient;
            _emailTemplateRepo = messageTemplateRepository;
            _log = eventLog;
            _passwordRepo = passwordResetRequestRepository;
            _profileRepo = userProfileRepository;
            _uow = unitOfWork;
        }
        #endregion

        #region methods
        public void GenerateNewUserPasswordLink(string emailAddress) {
            if(!ProfileHelper.IsInternalAddress(emailAddress)) {
                var profile = _profileRepo.GetCSProfile(emailAddress);

                if(profile != null) {
                    var token = Crypto.GenerateRandomToken();
                    var passwordRequest = new PasswordResetRequest() {
                        UserId = Guid.Parse(profile.Id),
                        Token = token,
                        Expiration = DateTime.UtcNow.AddYears(5) //Don't want this link to expire for new accounts, but 5 years is plenty of time
                    };

                    _passwordRepo.Create(passwordRequest);

                    _uow.SaveChanges();

                    SendResetPasswordEmail(emailAddress, token, newUserEmailTemplateKey, "setpassword");
                }
            }
        }

        public void GeneratePasswordResetLink(string emailAddress) {
            if(!ProfileHelper.IsInternalAddress(emailAddress)) {
                var profile = _profileRepo.GetCSProfile(emailAddress);

                if(profile != null) {
                    var token = Crypto.GenerateRandomToken();
                    var passwordRequest = new PasswordResetRequest() {
                        UserId = Guid.Parse(profile.Id),
                        Token = token,
                        Expiration = DateTime.UtcNow.AddDays(3) //Request is valid for 3 days
                    };

                    _passwordRepo.Create(passwordRequest);

                    _uow.SaveChanges();

                    SendResetPasswordEmail(emailAddress, token, resetEmailTemplateKey, "forgotpassword");
                }
            }
        }

        public ValidateTokenReturn IsTokenValid(string token) {
            ValidateTokenReturn ret = new ValidateTokenReturn();

            var passwordRequest = _passwordRepo.Read(p => p.Token.Equals(token) && !p.Processed && p.Expiration > DateTime.UtcNow).FirstOrDefault();

            if(passwordRequest == null) {
                return null;
            } else {
                var profile = _profileRepo.GetCSProfile(passwordRequest.UserId);

                if(profile == null) {
                    return ret;
                } else
                {
                    ret.Email = profile.Email;
                    ret.FirstName = profile.FirstName;
                    ret.LastName = profile.LastName;
                    ret.UserName = profile.Email.Substring(profile.Email.IndexOf('@') + 1);
                    ret.IsCorrect = true;

                    return ret;
                }
            }
        }

        public bool ResetPassword(ResetPasswordModel resetPassword) {
            try {
                //Retrieve the reset request, and verify that it's still valid
                var request = _passwordRepo.Read(p => p.Token.Equals(resetPassword.Token)).FirstOrDefault();

                if(request == null || request.Processed || request.Expiration < DateTime.UtcNow)
                    return false; //This is an invalid token

                var profile = _profileRepo.GetCSProfile(request.UserId);

                if(profile == null) {
                    return false;
                } else {
                    _adRepo.UpdatePassword(profile.Email, resetPassword.Password);
                    _adRepo.UnlockAccount(profile.Email);

                    request.Processed = true;
                    _passwordRepo.Update(request);
                    _uow.SaveChanges();

                    return true;
                }
            } catch(Exception ex) {
                _log.WriteErrorLog("Password reset error", ex);
                return false;
            }
        }

        private void SendResetPasswordEmail(string emailAddress, string token, string templateKey, string baseURL) {
            var template = _emailTemplateRepo.Read(m => m.TemplateKey.Equals(templateKey)).FirstOrDefault();

            if(template == null)
                throw new Exception("Reset Password Request email template not found");

            _emailClient.SendTemplateEmail(template.ToMessageTemplateModel(), 
                                           new List<string>() { emailAddress }, 
                                           new { resetLink = string.Format("{0}/#/{1}/?t={2}", Configuration.EntreeSiteURL, baseURL, Uri.EscapeDataString(token)) });
        }
        #endregion
    }
}
