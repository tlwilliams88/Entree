﻿CREATE PROCEDURE [ETL].[ReadBranches]
	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT * FROM [ETL].Staging_Branch WHERE LocationTypeId=3
END