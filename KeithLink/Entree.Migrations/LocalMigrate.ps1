param([int]$Version=0)


if ($Version -gt 0) {
	Write-Output("Migrating to version: " + $Version.ToString())
	& ..\..\..\packages\FluentMigrator.Tools.1.6.2\tools\AnyCPU\40\Migrate.exe /verbose "1" /conn "Data Source=localhost;Initial Catalog= BEK_Commerce_AppData;Integrated Security=SSPI;" /assembly "Entree.Migrations.dll" /provider sqlserver2012 /profile "Debug" /version $Version
} else {
	Write-Output("Migrating to head")
	& ..\..\..\packages\FluentMigrator.Tools.1.6.2\tools\AnyCPU\40\Migrate.exe /verbose "1" /conn "Data Source=localhost;Initial Catalog= BEK_Commerce_AppData;Integrated Security=SSPI;" /assembly "Entree.Migrations.dll" /provider sqlserver2012 /profile "Debug"
}
