/****** Object:  StoredProcedure [ETL].[ReadCSUsers]    Script Date: 10/27/2016 1:05:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [ETL].[ReadCSUsers]
AS

/*******************************************************************
* PROCEDURE: usp_ECOM_SelectCSUsers
* PURPOSE: Select distinct commerce server users and email addresses
* NOTES: {special set up or requirements, etc.}
* CREATED:	Jason McMillan 2014-10-10
* MODIFIED 
* DATE		AUTHOR			DESCRIPTION
*-------------------------------------------------------------------
* {date}	{developer}	{brief modification description}
*******************************************************************/

SET NOCOUNT ON

SELECT
	DISTINCT
	u_user_id
FROM
	[BEK_Commerce_profiles]..[UserObject]


GO
