using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entree.Migrations.Migrations.Helpers
{
    public static class ScriptsCollectionHelper
    {
        /// <summary>
        /// Get all the migrations for a specific migration number
        /// </summary>
        /// <param name="migrationNumber">0000 must be at least a length of 4</param>
        /// <returns></returns>
        public static List<string> GetAllMigrationFiles(string migrationNumber) {
            List<string> returnValue = new List<string>();

            // Make sure the formatting length is what we expect
            if (migrationNumber.Length < 4)
                throw new ApplicationException(string.Format("Migration {0} is not valid. A four or more character string is required"));

            returnValue.AddRange(Directory.GetFiles(@"Scripts\", string.Format("_{0}_", migrationNumber), SearchOption.AllDirectories).ToList<string>());

            return returnValue;
        }
    }
}
