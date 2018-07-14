using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entree.Core.Interface.Profile {
    public interface IAvatarRepository {
        bool SaveAvatar(Guid userId, string fileName, string base64String);
    }
}
