using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using KeithLink.Svc.Core.Models.Lists;

namespace KeithLink.Svc.Core.Extensions.Lists
{
    public static class IEnumerableExtensions
    {
        public static void TryAdd<T>(this List<T> self, T value) {
            if (value != null) {
                self.Add(value);
            }
        }

        public static void TryAddRange<T>(this List<T> self, List<T> value) {
            if (value != null) {
                self.AddRange(value);
            }
        }
    }
}
