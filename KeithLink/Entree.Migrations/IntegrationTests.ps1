param([int]$rollback=0)


if ($rollback -gt 0) {
	Write-Output("Rolling back all migrations")
	& ..\..\..\packages\FluentMigrator.Tools.1.6.2\tools\AnyCPU\40\Migrate.exe /verbose "1" /conn "Data Source=localhost;Initial Catalog= BEK_Commerce_AppData_test;Integrated Security=SSPI;" /assembly "Entree.Migrations.dll" /provider sqlserver2012 /profile "IntegrationTests" /context "BaselineSetup" /task rollback:all
} else {
	Write-Output("Migrating to head")
	& ..\..\..\packages\FluentMigrator.Tools.1.6.2\tools\AnyCPU\40\Migrate.exe /verbose "1" /conn "Data Source=localhost;Initial Catalog= BEK_Commerce_AppData_test;Integrated Security=SSPI;" /assembly "Entree.Migrations.dll" /provider sqlserver2012 /profile "IntegrationTests" /context "BaselineSetup"
}
