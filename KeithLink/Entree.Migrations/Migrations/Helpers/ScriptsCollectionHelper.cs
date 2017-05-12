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
        public static Queue<string> GetAllMigrationFiles(string migrationNumber, string direction) {
            Queue<string> returnValue = new Queue<string>();

            // Make sure the formatting length is what we expect
            if (migrationNumber.Length < 4)
                throw new ApplicationException(string.Format("Migration {0} is not valid. A four or more character string is required"));

            if (direction != "up" || direction != "down")
                throw new ApplicationException(string.Format("Cannot migration in {0} direction. Expecint 'up' or 'down'", direction));

            if (Directory.Exists(@"Scripts\") == false) {
                throw new FileNotFoundException("Directory 'Scripts\' not found.");
            }


            /*
             *  We will need to run the scripts in a specific order to avoid dependency issues
             *  The order we will execute in is:
             *     - Schemas
             *     - Types
             *     - Tables
             *     - Functions
             *     - Stored Procedures
             *     - Views
             *     - Indexes
             *     - Constraints
             */
            List<string> filesGlob = Directory.GetFiles(@"Scripts\", string.Format("*_{0}_*{1}.sql", migrationNumber, direction), SearchOption.AllDirectories).ToList<string>();


            // Add all schemas by ruling out all other script types
            filesGlob.Where(x =>
                            x.Contains("Table") == false &&
                            x.Contains("Functions") == false &&
                            x.Contains("Stored Procedures") == false &&
                            x.Contains("Types") == false &&
                            x.Contains("Seed") == false &&
                            x.Contains("Indexes") == false &&
                            x.Contains("Views") == false &&
                            x.Contains("Constraints") == false).ToList<string>().ForEach(x => returnValue.Enqueue(x));


            filesGlob.Where(x => x.Contains("Types")).ToList<string>().ForEach(x => returnValue.Enqueue(x));

            // Ignore Constraints and Indexes - they are to be processed later
            filesGlob.Where(x => x.Contains("Tables") && 
                                 x.Contains("Constraints") == false && 
                                 x.Contains("Indexes") == false).ToList<string>().ForEach(x => returnValue.Enqueue(x));

            filesGlob.Where(x => x.Contains("Functions")).ToList<string>().ForEach(x => returnValue.Enqueue(x));
            filesGlob.Where(x => x.Contains("Stored Procedures")).ToList<string>().ForEach(x => returnValue.Enqueue(x));
            filesGlob.Where(x => x.Contains("Views")).ToList<string>().ForEach(x => returnValue.Enqueue(x));
            filesGlob.Where(x => x.Contains("Indexes")).ToList<string>().ForEach(x => returnValue.Enqueue(x));
            filesGlob.Where(x => x.Contains("Constraints")).ToList<string>().ForEach(x => returnValue.Enqueue(x));


            return returnValue;
        }
    }
}
