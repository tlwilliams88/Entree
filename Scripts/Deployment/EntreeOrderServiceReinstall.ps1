# Variables
$appName = "Entree Order Service"

$installer = "C:\MIS\KeithLink.Svc.Windows.OrderService.Setup.msi"
$installLocation = "C:\Program Files (x86)\Ben E. Keith Company\Entree Order Service"

$configBackupLocation = "C:\Program Files (x86)\Ben E. Keith Company\"
$config = "C:\Program Files (x86)\Ben E. Keith Company\KeithLink.Svc.Windows.OrderService.exe.config"

# Backup Config Before Starting
Write-Host "Backing up configuration file..."
Copy-Item $config $installLocation\..\

# Stopping the service
Write-Host "Stopping service..."
Stop-Service -displayname $appName

# Uninstall the current application
Write-Host "Uninstalling Service..."
$app = Get-WmiObject -Class Win32_Product | Where-Object { 
    $_.Name -match "Entree Order Service"
}

if ($app) {
    $app.Uninstall()
}

# Install new package
Write-Host "Installing new service..."
Start-Process $installer /qn -Wait

# Update config
Write-Host "Updating config..."
Copy-Item $config $installLocation


Write-Host "Finished"