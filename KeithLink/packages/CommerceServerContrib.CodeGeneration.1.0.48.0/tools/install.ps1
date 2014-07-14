param($installPath, $toolsPath, $package, $project)
# Remove the custom tool settings from things that are not run by themselves

# Entity Child Template 
$childTemplate = $project.ProjectItems.Item("Models").ProjectItems.Item("CommerceEntityTemplate.tt")

# Clear the custom tool setting
$customToolSetting = $childTemplate.Properties.Item("CustomTool")
$customToolSetting.Value = ""

# Request Template Child Template 
$rtChildTemplate = $project.ProjectItems.Item("RequestTemplates").ProjectItems.Item("RequestTemplateTemplate.tt")

$rtCustomToolSetting = $rtChildTemplate.Properties.Item("CustomTool")
$rtCustomToolSetting.Value = ""

# Include File
$includeChildTemplate = $project.ProjectItems.Item("TemplateGenerationCommon").ProjectItems.Item("CodeGenerationIncludes.tt")

$includeCustomToolSetting = $includeChildTemplate.Properties.Item("CustomTool")
$includeCustomToolSetting.Value = ""

<#
	The T4 tool runs when the tt file is added to the project.  We remove the custom tool property from 
	the child template but want it to remain for the parent template.  Since it will not run correctly the first time
	because of the broken assembly reference until the project has been built the install script will just remove the 
	first output.
#>

$modelGenerator = $project.ProjectItems.Item("Models").ProjectItems.Item("CommerceEntities.tt").ProjectItems.Item("CommerceEntities.cs")
$modelGenerator.Delete()

$requestTemplates = $project.ProjectItems.Item("RequestTemplates").ProjectItems.Item("RequestTemplates.tt").ProjectItems.Item("RequestTemplates.cs")
$requestTemplates.Delete()


