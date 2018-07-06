
using KeithLink.Svc.Core.Interface.Documents;
using KeithLink.Svc.Core.Interface.Profile;
using KeithLink.Svc.Core.Models.Documents;
using System.Collections.Generic;
using System.Web.Http;

namespace KeithLink.Svc.WebApi.Controllers
{
    public class DocumentsController : BaseController {
        #region attributes
        private readonly IAzureContainerRepository _docRepo;
        private readonly IUserProfileLogic _profileLogic;

        #endregion

        #region ctor
        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="profileLogic"></param>

        public DocumentsController(IAzureContainerRepository docRepo, IUserProfileLogic profileLogic) : base(profileLogic) {
            _docRepo = docRepo;
            _profileLogic = profileLogic;
        }
        #endregion

        #region methods
        /// <summary>
        /// get a specific feature flag's value
        /// </summary>
        /// <param name="identifier"></param>
        /// <returns></returns>
        [Authorize]
        [HttpGet]
        [ApiKeyedRoute("documents")]
        public List<DocumentReturnModel> GetDocuments([FromUri] string identifier)
        {
            return _docRepo.GetAllDocuments(identifier);
        }       
        #endregion
    }
}
