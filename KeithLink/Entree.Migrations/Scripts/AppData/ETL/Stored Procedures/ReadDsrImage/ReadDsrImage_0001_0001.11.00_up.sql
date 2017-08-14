/****** Object:  StoredProcedure [ETL].[ReadDsrImage]    Script Date: 10/27/2016 1:05:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [ETL].[ReadDsrImage] AS

/*******************************************************************
* PROCEDURE: ReadDsrImage
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
	LTRIM(RTRIM(LOWER(d.EmailAddress))) 'EmailAddress'
	, e.EMPLOYEE_PHOTO 'EmployeePhoto'
FROM
	ETL.Staging_EmployeeInfo e
	INNER JOIN ETL.Staging_Dsr d ON LTRIM(RTRIM(LOWER(e.EMAIL_ADDR))) = LTRIM(RTRIM(LOWER(d.EmailAddress)))


GO
