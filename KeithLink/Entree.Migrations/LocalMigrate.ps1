param([int]$rollback=0)

if ($rollback -gt 0) {
	Write-Output("Migrating to previous version 1 step")
	& ..\..\..\packages\FluentMigrator.Tools.1.6.2\tools\AnyCPU\40\Migrate.exe /verbose "1" /conn "Data Source=localhost;Initial Catalog= BEK_Commerce_AppData;Integrated Security=SSPI;" /assembly "Entree.Migrations.dll" /provider sqlserver2012 /profile "Debug" /task rollback --timeout=300
} else {
	Write-Output("Migrating to head")
	& ..\..\..\packages\FluentMigrator.Tools.1.6.2\tools\AnyCPU\40\Migrate.exe /verbose "1" /conn "Data Source=localhost;Initial Catalog= BEK_Commerce_AppData;Integrated Security=SSPI;Connection Timeout=0;" /assembly "Entree.Migrations.dll" /provider sqlserver2012 /profile "Debug"
}
