_____________________________________________________________________________________________

THIS FILE INCLUDES INSTRUCTIONS FOR SETTING UP THE COMPONENTS
NECESSARY FOR THE KeithLink.ETL\ETLProcess.dtsx PACKAGE TO EXECUTE SUCCESSFULLY
_____________________________________________________________________________________________

----------------------------------------------------------------------------------------------------------------
SQL Server Prerequisites
----------------------------------------------------------------------------------------------------------------
--DEVELOPMENT:
Prerequitsites for Development/Test (No-matter which machine SSIS package is running on):
	1. On TEST/DEV SQL Server instance containing database "FSE" (Currently corpsqltst12),
	   run the following SQL scripts:
		a. Create_usp_ECOM_SelectProductAllergens.sql
		b. Create_usp_ECOM_SelectProductDiet.sql
		c. Create_usp_ECOM_SelectProductNutrition.sql
		d. Create_usp_ECOM_SelectProductSpec.sql
		e. Create_function_GetItemType.sql

--PRODUCTION
Prerequitsites for Production (No-matter which machine SSIS package is running on):
	1. On PRODUCTION SQL Server instance containing database "FSE" (Currently bekcpsql),
	   run the following SQL scripts (Coordinate with BEK DBA's):
		a. Create_usp_ECOM_SelectProductAllergens.sql
		b. Create_usp_ECOM_SelectProductDiet.sql
		c. Create_usp_ECOM_SelectProductNutrition.sql
		d. Create_usp_ECOM_SelectProductSpec.sql
		e. Create_function_GetItemType.sql

----------------------------------------------------------------------------------------------------------------
Machine Setup Prerequisites
----------------------------------------------------------------------------------------------------------------
LOCAL --To Execute SSIS package on LOCAL MACHINE:
	1. Execute the following SQL scripts on your local machine SQL instance
		a. Create_BEK_Commerce_AppData.sql
		b. Create_SSISSupport_ECOM.sql
	2. Create the following directories (if they don't already exist)
		a. C:\SSIS_XML\ECOM
		b. C:\SSIS_WEB\ECOM\WSDL
	3. Copy the ETLProcess.dtsConfig.Local file to your local C:\SSIS_XML\ECOM directory
	4. Rename the ETLProcess.dtsConfig.Local file to ETLProcess.dtsConfig
	5. Modify your local copy of ETLProcess.dtsConfig so that the database, flat file, and web service
	   connections are pointing to your local deployments (local SQL Server, wherever you've stored customer flat file,
       and locally deployed web service)
	6. You should now be able to execute the ETLProcess.dtsx package locally

DEVELOPMENT --To Execute SSIS package on DEVELOPMENT/TEST SERVER:
	1. Execute the following SQL scripts on the development SQL database instance (Coordinate with BEK DBA's)
		a. Create_BEK_Commerce_AppData.sql
	2. Excute the following SQL scripts on the development SQL SSIS package store (Coordinate with BEK DBA's)
		a. Create_SSISSupport_ECOM.sql
	3. Create the following directories on the development SQL SSIS server (if they don't already exist; Coordinate with BEK DBA's)
		a. C:\SSIS_XML\ECOM
		b. C:\SSIS_WEB\ECOM\WSDL
	4. Copy the ETLProcess.dtsConfig.Prod file to the C:\SSIS_XML\ECOM directory
	5. Rename the ETLProcess.dtsConfig.Prod file to ETLProcess.dtsConfig
	6. Ensure the database, flat file and web service configurations are connecting to the production environment
    7. You should now be able to execute the ETLProcess.dtsx package on the development server
		Note:  The ETLProcess.dtsx package will be executed by a SQL agent job.  Coordinate with BEK DBA's to 
			   get this job setup adn scheduled appropriately..

PRODUCTION --To Execute SSIS package on PRODUCTION SERVER:
	1. Execute the following SQL scripts on the production SQL database instance (Coordinate with BEK DBA's)
		a. Create_BEK_Commerce_AppData.sql
	2. Execute the following SQL scripts on the production SQL SSIS package store (Coordinate with BEK DBA's)
		a. Create_SSISSupport_ECOM.sql
	3. Create the following directories on the production SQL SSIS server (if they don't already exist; Coordinate with BEK DBA's)
		a. C:\SSIS_XML\ECOM
		b. C:\SSIS_WEB\ECOM\WSDL
	4. Copy the ETLProcess.dtsConfig.Prod file to the C:\SSIS_XML\ECOM directory
	5. Rename the ETLProcess.dtsConfig.Prod file to ETLProcess.dtsConfig
	6. Ensure the database, flat file and web service configurations are connecting to the production environment
    7. You should now be able to execute the ETLProcess.dtsx package on the development server
		Note:  The ETLProcess.dtsx package will be executed by a SQL agent job.  Coordinate with BEK DBA's to 
			   get this job setup adn scheduled appropriately.