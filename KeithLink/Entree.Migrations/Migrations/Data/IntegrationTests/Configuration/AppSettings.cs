using System;
using BEK.FluentMigratorBase; 
using FluentMigrator;

namespace Entree.Migrations.Migrations.Data.IntegrationTests.Configuration {
    [Profile("IntegrationTests")]
    public class TestAppSettings : Migration {
        public override void Up() {
            this.Execute.Script(@"Scripts\AppData\Configuration\Tables\AppSettings\PostDeployment\Configuration.AppSettings.Bootstrap.local.sql");
        }

        public override void Down() {
            throw new NotImplementedException();
        }
    }
}
