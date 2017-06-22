using System;
using System.Reflection;

using FluentMigrator;
using FluentMigrator.Runner;
using FluentMigrator.Runner.Announcers;
using FluentMigrator.Runner.Initialization;

namespace KeithLink.Svc.Impl.Tests.Integration {
    public class Migrator {
        string connectionString;

        public Migrator(string connectionString) {
            this.connectionString = connectionString;
        }

        private class MigrationOptions : IMigrationProcessorOptions {
            public bool PreviewOnly { get; set; }

            public int Timeout { get; set; }

            public string ProviderSwitches { get; }
        }

        public void Migrate(Assembly assembly, Action<IMigrationRunner> runnerAction) {
            var options = new MigrationOptions {PreviewOnly = false, Timeout = 0};
            var factory = new FluentMigrator.Runner.Processors.SqlServer.SqlServer2012ProcessorFactory();

            var announcer = new TextWriterAnnouncer(s => System.Diagnostics.Debug.WriteLine(s));

            var migrationContext = new RunnerContext(announcer) {
                Profile = "IntegrationTests",
                ApplicationContext = "BaselineSetup"
            };
            
            var processor = factory.Create(this.connectionString, announcer, options);
            var runner = new MigrationRunner(assembly, migrationContext, processor);

            runnerAction(runner);
        }
    }
}