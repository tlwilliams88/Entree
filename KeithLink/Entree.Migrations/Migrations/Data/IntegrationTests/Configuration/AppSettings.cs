﻿using System;
using FluentMigrator;

namespace Entree.Migrations.Migrations.Data.IntegrationTests.Configuration {
    [Profile("IntegrationTests")]
    public class TestAppSettings : Migration {
        public override void Up() {
            this.Execute.Script(@"SQL\Configs\Configuration.AppSettings.Bootstrap.local.sql");
        }

        public override void Down() {
            throw new NotImplementedException();
        }
    }
}
