﻿CREATE PROCEDURE [List].[GetCustomListShare]
	@Id     BIGINT
AS
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	SELECT
	    [Id],
	    [BranchId],
	    [CustomerNumber],
	    [HeaderId],
        [Active],
	    [CreatedUtc],
	    [ModifiedUtc]
	FROM 
        [List].[CustomListShares] 
	WHERE	
        [Id] = @Id