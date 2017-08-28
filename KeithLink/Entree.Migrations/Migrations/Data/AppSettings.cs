﻿using BEK.FluentMigratorBase; 
using FluentMigrator;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entree.Migrations.Data {
    [Profile("Debug")]
    public class AppSettings : Migration {
        #region attributes
        private Dictionary<string, string> _specificConfigurationsToRun;
        #endregion

        public AppSettings() {
            _specificConfigurationsToRun = new Dictionary<string, string>();

            // Matt
            _specificConfigurationsToRun.Add("CORPMISDEV2A", @"SQL\Configs\matt.local.configuration.sql");

            // Jeremy
            _specificConfigurationsToRun.Add("CORPMISDEV2B", @"SQL\Configs\jeremy.local.configuration.sql");

            // Brett
            _specificConfigurationsToRun.Add("CORPMISDEV2H", @"SQL\Configs\bakillins.local.configuration.sql");
        }

        public override void Up() {
            try {
                this.Execute.Script(_specificConfigurationsToRun[System.Environment.MachineName]);
            }
            catch (Exception ex) {
                Exception configException = new Exception(System.Environment.MachineName, ex);
                throw configException;
            }
        }

        public override void Down() {
            throw new NotImplementedException();
        }
    }
}
