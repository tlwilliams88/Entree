using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entree.Migrations.Helpers
{
    public static class ScriptsCollectionHelper
    {
        #region attributes
        private const int NOT_FOUND = -1;

        private const string PARM_DIRECTION_DOWN = "down";
        private const string PARM_DIRECTION_UP = "up";

        private const string SCHEMA_OBJECT_CONSTRAINTS = "Constraints";
        private const string SCHEMA_OBJECT_FUNCTIONS = "Functions";
        private const string SCHEMA_OBJECT_INDEXES = "Indexes";
        private const string SCHEMA_OBJECT_POSTDEPLOYMENTS = "PostDeployment";
        private const string SCHEMA_OBJECT_SEEDS = "Seed";
        private const string SCHEMA_OBJECT_STOREDPROCEDURES = "Stored Procedures";
        private const string SCHEMA_OBJECT_TABLES = "Tables";
        private const string SCHEMA_OBJECT_TYPES = "Types";
        private const string SCHEMA_OBJECT_VIEWS = "Views";
        #endregion

        #region methods
        private static Queue<string> BuildDownwardMigrationQueue(string migrationNumber) {
            /*
             *  We will need to run the scripts in a specific order to avoid dependency issues
             *  The order we will execute in is:
             *     - Stored Procedures
             *     - Functions
             *     - Views
             *     - Constraints
             *     - Indexes
             *     - Tables
             *     - Types
             *     - Schemas
             */
            List<string> filesGlob = Directory.GetFiles(@"Scripts\", $"*_{migrationNumber}_*down.sql", SearchOption.AllDirectories).ToList<string>();

            Queue<string> returnValue = new Queue<string>();

            GetObjectList(SCHEMA_OBJECT_POSTDEPLOYMENTS, filesGlob)
                .ForEach(x => returnValue.Enqueue(x));

            GetObjectList(SCHEMA_OBJECT_STOREDPROCEDURES, filesGlob)
                .ForEach(x => returnValue.Enqueue(x));

            GetObjectList(SCHEMA_OBJECT_FUNCTIONS, filesGlob)
                .ForEach(x => returnValue.Enqueue(x));

            GetObjectList(SCHEMA_OBJECT_VIEWS, filesGlob)
                .ForEach(x => returnValue.Enqueue(x));

            GetObjectList(SCHEMA_OBJECT_CONSTRAINTS, filesGlob)
                .ForEach(x => returnValue.Enqueue(x));

            GetObjectList(SCHEMA_OBJECT_INDEXES, filesGlob)
                .ForEach(x => returnValue.Enqueue(x));

            GetTableList(filesGlob)
                .ForEach(x => returnValue.Enqueue(x));

            GetObjectList(SCHEMA_OBJECT_TYPES, filesGlob)
                .ForEach(x => returnValue.Enqueue(x));

            GetSchemaList(filesGlob)
                .ForEach(x => returnValue.Enqueue(x));

            return returnValue;
        }

        private static Queue<string> BuildUpwardMigrationQueue(string migrationNumber) {
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
            List<string> filesGlob = Directory.GetFiles(@"Scripts\", $"*_{migrationNumber}_*up.sql", SearchOption.AllDirectories).ToList<string>();

            Queue<string> returnValue = new Queue<string>();

            GetSchemaList(filesGlob)
                .ForEach(x => returnValue.Enqueue(x));

            GetObjectList(SCHEMA_OBJECT_TYPES, filesGlob)
                .ForEach(x => returnValue.Enqueue(x));

            GetTableList(filesGlob)
                .ForEach(x => returnValue.Enqueue(x));

            GetObjectList(SCHEMA_OBJECT_FUNCTIONS, filesGlob)
                .ForEach(x => returnValue.Enqueue(x));

            GetObjectList(SCHEMA_OBJECT_STOREDPROCEDURES, filesGlob)
                .ForEach(x => returnValue.Enqueue(x));

            GetObjectList(SCHEMA_OBJECT_VIEWS, filesGlob)
                .ForEach(x => returnValue.Enqueue(x));

            GetObjectList(SCHEMA_OBJECT_INDEXES, filesGlob)
                .ForEach(x => returnValue.Enqueue(x));

            GetObjectList(SCHEMA_OBJECT_CONSTRAINTS, filesGlob)
                .ForEach(x => returnValue.Enqueue(x));

            GetObjectList(SCHEMA_OBJECT_POSTDEPLOYMENTS, filesGlob)
                .ForEach(x => returnValue.Enqueue(x));

            return returnValue;
        }

        /// <summary>
        /// Get all the migrations for a specific migration number
        /// </summary>
        /// <param name="migrationNumber">0000 must be at least a length of 4</param>
        /// <returns></returns>
        public static Queue<string> GetAllMigrationFiles(string migrationNumber, string direction) {
            // Make sure the formatting length is what we expect
            if(migrationNumber.Length < 4)
                throw new ApplicationException($"Migration {migrationNumber} is not valid. A four or more character string is required");

            if(!direction.Equals(PARM_DIRECTION_DOWN, StringComparison.CurrentCulture) &&
               !direction.Equals(PARM_DIRECTION_UP, StringComparison.CurrentCulture))
                throw new ApplicationException($"Cannot migration in {direction} direction. Expect 'up' or 'down'");

            if(Directory.Exists(@"Scripts\") == false) {
                throw new FileNotFoundException("Directory 'Scripts\' not found.");
            }

            Queue<string> returnValue = new Queue<string>();

            if(direction.Equals(PARM_DIRECTION_DOWN, StringComparison.CurrentCulture)) {
                returnValue = BuildDownwardMigrationQueue(migrationNumber);
            }
            if(direction.Equals(PARM_DIRECTION_UP, StringComparison.CurrentCulture)) {
                returnValue = BuildUpwardMigrationQueue(migrationNumber);
            }

            return returnValue;
        }

        private static string GetDirectoryName(string objectTypeName) {
            StringBuilder pathBuilder = new StringBuilder();

            pathBuilder.Append(@"\");
            pathBuilder.Append(objectTypeName);
            pathBuilder.Append(@"\");

            return pathBuilder.ToString();
        }

        private static List<string> GetObjectList(string objectTypeName, List<string> fileList) {
            return fileList.Where(x => x.IndexOf(GetDirectoryName(objectTypeName), StringComparison.OrdinalIgnoreCase) > NOT_FOUND)
                           .ToList<string>();
        }

        private static List<string> GetSchemaList(List<string> fileList) {
            // Add all schemas by ruling out all other script types
            return fileList.Where(x =>
                                      x.IndexOf(GetDirectoryName(SCHEMA_OBJECT_TABLES), StringComparison.OrdinalIgnoreCase) == NOT_FOUND &&
                                      x.IndexOf(GetDirectoryName(SCHEMA_OBJECT_FUNCTIONS), StringComparison.OrdinalIgnoreCase) == NOT_FOUND &&
                                      x.IndexOf(GetDirectoryName(SCHEMA_OBJECT_STOREDPROCEDURES), StringComparison.OrdinalIgnoreCase) == NOT_FOUND &&
                                      x.IndexOf(GetDirectoryName(SCHEMA_OBJECT_TYPES), StringComparison.OrdinalIgnoreCase) == NOT_FOUND &&
                                      x.IndexOf(GetDirectoryName(SCHEMA_OBJECT_SEEDS), StringComparison.OrdinalIgnoreCase) == NOT_FOUND &&
                                      x.IndexOf(GetDirectoryName(SCHEMA_OBJECT_INDEXES), StringComparison.OrdinalIgnoreCase) == NOT_FOUND &&
                                      x.IndexOf(GetDirectoryName(SCHEMA_OBJECT_VIEWS), StringComparison.OrdinalIgnoreCase) == NOT_FOUND &&
                                      x.IndexOf(GetDirectoryName(SCHEMA_OBJECT_CONSTRAINTS), StringComparison.OrdinalIgnoreCase) == NOT_FOUND &&
                                      x.IndexOf(GetDirectoryName(SCHEMA_OBJECT_POSTDEPLOYMENTS), StringComparison.OrdinalIgnoreCase) == NOT_FOUND)
                           .ToList<string>();
        }

        private static List<string> GetTableList(List<string> fileList) {
            // Ignore Constraints and Indexes - they are to be processed later
            return fileList.Where(x => x.IndexOf(GetDirectoryName(SCHEMA_OBJECT_TABLES), StringComparison.OrdinalIgnoreCase) > NOT_FOUND &&
                                       x.IndexOf(GetDirectoryName(SCHEMA_OBJECT_CONSTRAINTS), StringComparison.OrdinalIgnoreCase) == NOT_FOUND &&
                                       x.IndexOf(GetDirectoryName(SCHEMA_OBJECT_INDEXES), StringComparison.OrdinalIgnoreCase) == NOT_FOUND &&
                                       x.IndexOf(GetDirectoryName(SCHEMA_OBJECT_SEEDS), StringComparison.OrdinalIgnoreCase) == NOT_FOUND &&
                                       x.IndexOf(GetDirectoryName(SCHEMA_OBJECT_POSTDEPLOYMENTS), StringComparison.OrdinalIgnoreCase) == NOT_FOUND)
                           .ToList<string>();
        }
        #endregion
    }
}
