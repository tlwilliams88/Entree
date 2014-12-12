using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KeithLink.Svc.Core;
using KeithLink.Svc.Core.Interface.Profile;
using KeithLink.Svc.Core.Models.Profile;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;

namespace KeithLink.Svc.Impl.Repository.Profile {
    public class AvatarRepositoryImpl:IAvatarRepository {

        public AvatarRepositoryImpl() {
        }

        public void SaveAvatar( Guid userId, string fileName, string base64String ) {
            // Do stuff!
        }

    }
}
