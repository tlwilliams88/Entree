
CREATE PROCEDURE [ETL].[ReadDsrInfo] AS

/*******************************************************************
* PROCEDURE: ReadDsrInfo
* PURPOSE: 
* NOTES: {special set up or requirements, etc.}
* CREATED:	Jason McMillan 1/28/2015
* MODIFIED 
* DATE		AUTHOR			DESCRIPTION
*-------------------------------------------------------------------
* {date}	{developer}	{brief modification description}
*******************************************************************/

SET NOCOUNT ON;

SELECT
	RIGHT(LTRIM(RTRIM(d.DsrNumber)), 3) 'DsrNumber'
	, LTRIM(RTRIM(LOWER(d.EmailAddress))) 'EmailAddress'
	, CASE LEFT(d.DsrNumber, 2)
		WHEN 'AM' THEN 'FAM'
		WHEN 'AQ' THEN 'FAQ'
		WHEN 'AR' THEN 'FAR'
		WHEN 'DF' THEN 'FDF'
		WHEN 'HS' THEN 'FHS'
		WHEN 'LR' THEN 'FLR'
		WHEN 'OK' THEN 'FOK'
		WHEN 'SA' THEN 'FSA'
		WHEN 'ZN' THEN 'ZZZZZZZ'
	END 'BranchId'
	, CONCAT(SUBSTRING(Name, CHARINDEX(',',Name,0) + 1, LEN(Name) - CHARINDEX(',',Name,0)), ' ', SUBSTRING(Name, 0, CHARINDEX(',',Name,0))) 'Name'
	, REPLACE(REPLACE(LTRIM(RTRIM(e.PHONE)),'/',''),'-','') 'Phone'
	, CONCAT('{baseUrl}/userimages/',LTRIM(RTRIM(LOWER(d.EmailAddress)))) 'ImageUrl'
FROM
	ETL.Staging_EmployeeInfo e
	INNER JOIN ETL.Staging_Dsr d ON LTRIM(RTRIM(LOWER(e.EMAIL_ADDR))) = LTRIM(RTRIM(LOWER(d.EmailAddress)))