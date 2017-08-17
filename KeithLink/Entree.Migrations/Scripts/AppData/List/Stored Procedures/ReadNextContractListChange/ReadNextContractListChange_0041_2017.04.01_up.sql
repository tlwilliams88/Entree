ALTER PROCEDURE [List].[ReadNextContractListChange]
AS
BEGIN
	SELECT 
		lid.[CustomerNumber]
		, lid.[BranchId]
		, lid.[ItemNumber]
		, lid.[Each]
		, lid.[CreatedUtc]
		, lid.[ModifiedUtc]
		, lid.[CatalogId]
		, lid.[Status]
		FROM [List].[ListItemsDelta] lid
		WHERE lid.[CustomerNumber] = (SELECT top 1
								  	[CustomerNumber]
									FROM [List].[ListItemsDelta]
									WHERE [Sent] = 0)
            AND lid.[BranchId] = (  SELECT top 1
                                    [BranchId]
                                    FROM [List].[ListItemsDelta]
                                    WHERE [Sent] = 0)
			AND lid.[Sent] = 0
		ORDER BY lid.[Status]
END
GO