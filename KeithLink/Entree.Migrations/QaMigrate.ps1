param([int]$rollback=0)


if ($rollback -gt 0) {
	Write-Output("Migrating to previous version 1 step")
	& ..\..\..\packages\FluentMigrator.Tools.1.6.2\tools\AnyCPU\40\Migrate.exe /verbose "1" /conn "Data Source=corpkecqdb1;Initial Catalog= BEK_Commerce_AppData;User Id=ecom;Password=Qa!2klkA5" /assembly "Entree.Migrations.dll" /provider sqlserver2012 /task rollback /output /outputFilename "v1.12.0-migrations.sql" /previe
} else {
	Write-Output("Migrating to head")
	& ..\..\..\packages\FluentMigrator.Tools.1.6.2\tools\AnyCPU\40\Migrate.exe /verbose "1" /conn "Data Source=corpkecqdb1;Initial Catalog= BEK_Commerce_AppData;User Id=ecom;Password=Qa!2klkA5" /assembly "Entree.Migrations.dll" /provider sqlserver2012 /output /outputFilename "v1.12.0-migrations.sql" /previe
}
