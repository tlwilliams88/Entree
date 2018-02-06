param([int]$rollback=0)


if ($rollback -gt 0) {
	Write-Output("Migrating to previous version 1 step")
	& ..\..\..\packages\FluentMigrator.Tools.1.6.2\tools\AnyCPU\40\Migrate.exe /verbose "1" /conn "Data Source=corpsqltst14;Initial Catalog= BEK_Commerce_AppData;Integrated Security=SSPI;" /assembly "Entree.Migrations.dll" /provider sqlserver2012 /task rollback
} else {
	Write-Output("Migrating to head")
	& ..\..\..\packages\FluentMigrator.Tools.1.6.2\tools\AnyCPU\40\Migrate.exe /verbose "1" /conn "Data Source=corpsqltst14;Initial Catalog= BEK_Commerce_AppData;Integrated Security=SSPI;" /assembly "Entree.Migrations.dll" /provider sqlserver2012
}
