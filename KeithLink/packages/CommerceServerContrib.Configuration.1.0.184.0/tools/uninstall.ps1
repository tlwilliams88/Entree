param($installPath, $toolsPath, $package, $project)

# Load the configuration file
$configurationFile = $project.ProjectItems.Item("Web.config")
$configurationFilePath = $configurationFile.FileNames(1)

[xml]$config = Get-Content $configurationFilePath
[System.Xml.XmlNamespaceManager]$namespaceManager = $config.NameTable
$namespaceManager.AddNamespace("x", "urn:schemas-microsoft-com:asm.v1")

$runtime = $config.SelectSingleNode("/configuration/runtime/x:assemblyBinding", $namespaceManager)
$csAssembly = $runtime.SelectSingleNode("x:dependentAssembly/assemblyIdentity[@name='Microsoft.Practices.Unity']", $namespaceManager)

$runtime.RemoveChild($csAssembly.ParentNode)

$config.Save($configurationFilePath)


