$installLocation = "C:\Program Files (x86)\Commerce Server 10\Pipeline Components\"
$bin = "C:\Projects\entree\KeithLink\KeithLink.Ext.Pipeline.ItemPrice\bin\Debug"

$regasm = "C:\Windows\Microsoft.NET\Framework64\v4.0.30319\Regasm.exe"
$parameters = '/codebase', 'C:\Program Files (x86)\Commerce Server 10\Pipeline Components\KeithLink.Ext.Pipeline.ItemPrice.dll'

Write-Host "Copying files to Commerce Server 10"
Copy-Item $bin\* $installLocation

Write-Host "Installing pipeline..."
& $regasm $parameters

Write-Host "Done."