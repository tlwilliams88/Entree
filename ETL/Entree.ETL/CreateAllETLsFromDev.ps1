############################################################################
#  
#  CREATE ALL ETLs FROM DEV
#
#  This script will take the current directories _DEV version of the ETLProcess
#  and UNFIPRocess ETLs and duplicate them for every environment while changing
#  the referenced config and wsdl files for both.
#
#
#  Requirements: 
#         - _DEV versions of the ETL must exist in the directory this executes from
#         - Ensure that no ETLs have deactivated tasks
#
#  Author: mdjoiner
#  Last Updated: 2016-02-01
#
############################################################################


# Copy ETL Process replacing needed configs/names with the proper identifier
(get-content ETLProcess_DEV.dtsx) | foreach-object {$_ -replace "_DEV", "_QA"} | set-content -Encoding UTF8 ETLProcess_QA.dtsx
(get-content ETLProcess_DEV.dtsx) | foreach-object {$_ -replace "_DEV", "_BETA"} | set-content -Encoding UTF8 ETLProcess_BETA.dtsx
(get-content ETLProcess_DEV.dtsx) | foreach-object {$_ -replace "_DEV", "_RELEASE"} | set-content -Encoding UTF8 ETLProcess_RELEASE.dtsx


# Copy UNFI ETL Processes replacing needed configs/names with proper identifier
(get-content UNFIETLProcess_DEV.dtsx) | foreach-object {$_ -replace "_DEV", "_QA"} | set-content -Encoding UTF8 UNFIETLProcess_QA.dtsx
(get-content UNFIETLProcess_DEV.dtsx) | foreach-object {$_ -replace "_DEV", "_BETA"} | set-content -Encoding UTF8 UNFIETLProcess_BETA.dtsx
(get-content UNFIETLProcess_DEV.dtsx) | foreach-object {$_ -replace "_DEV", "_RELEASE"} | set-content -Encoding UTF8 UNFIETLProcess_RELEASE.dtsx