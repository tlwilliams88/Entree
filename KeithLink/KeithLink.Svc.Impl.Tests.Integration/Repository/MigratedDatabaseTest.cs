using System.Configuration;
using System.Reflection;

namespace KeithLink.Svc.Impl.Tests.Integration.Repository {
    public abstract class MigratedDatabaseTest {
        #region attributes
        private const string CONNECTIONSTRING_NAME = "BEKDBContext";
        #endregion

        #region ctor
        protected MigratedDatabaseTest() {
            Migrator dbMigrator = new Migrator(ConfigurationManager.ConnectionStrings[CONNECTIONSTRING_NAME].ConnectionString);

            // reset the database to its initial state
            dbMigrator.Migrate(Assembly.Load("Entree.Migrations"), runner => runner.RollbackToVersion(0));

            // run all of the migrations to get the database to the current version
            dbMigrator.Migrate(Assembly.Load("Entree.Migrations"), runner => runner.MigrateUp());
        }
        #endregion
    }
}
