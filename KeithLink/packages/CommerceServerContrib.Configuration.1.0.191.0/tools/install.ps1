param($installPath, $toolsPath, $package, $project)

# Load the configuration file
$configurationFile = $project.ProjectItems.Item("Web.config")
$configurationFilePath = $configurationFile.FileNames(1)

[xml]$config = Get-Content $configurationFilePath
$configurationNode = $config.SelectSingleNode("configuration")

if($configurationNode -ne $null)
{
	$runtime = $configurationNode.SelectSingleNode("runtime")
	if($runtime -eq $null)
	{
		$runtime = $configurationNode.CreateElement("runtime")
		$configurationNode.AppendChild($runtime)
	}

	[System.Xml.XmlNamespaceManager]$namespaceManager = $config.NameTable
	$namespaceManager.AddNamespace("x", "urn:schemas-microsoft-com:asm.v1")

	$assemblyBinding = $runtime.SelectSingleNode("x:assemblyBinding", $namespaceManager)
	if($assemblyBinding -eq $null)
	{
		$assemblyBinding = $config.CreateElement("assemblyBinding", "urn:schemas-microsoft-com:asm.v1")
		$runtime.AppendChild($assemblyBinding)
	}

	$dependentAssembly = $config.CreateElement("dependentAssembly", "urn:schemas-microsoft-com:asm.v1")
	$dependentAssembly.InnerXml = "<assemblyIdentity name=`"Microsoft.Practices.Unity`" publicKeyToken=`"31bf3856ad364e35`" culture=`"neutral`" /><bindingRedirect oldVersion=`"0.0.0.0-2.1.505.0`" newVersion=`"2.1.505.0`" />";
	$assemblyBinding.AppendChild($dependentAssembly)
	$config.Save($configurationFilePath)
}