using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Core.Interface.Profile {
    public interface IAvatarRepository {
        string GetAvatarUrl(Guid userId);

        void SaveAvatar(Guid userId, string fileName, string base64String);
    }
}
