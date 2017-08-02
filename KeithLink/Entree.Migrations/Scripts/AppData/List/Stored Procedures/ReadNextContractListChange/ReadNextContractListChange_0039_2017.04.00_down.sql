ALTER PROCEDURE [List].[ReadNextContractListChange]
AS
BEGIN
	SELECT 
		l.[CustomerId]
		, l.[BranchId]
		, lid.[ParentList_Id] AS [ParentList_Id]
		, lid.[ItemNumber]
		, lid.[Each]
		, lid.[CreatedUtc]
		, lid.[ModifiedUtc]
		, lid.[CatalogId]
		, lid.[Status]
		FROM [List].[ListItemsDelta] lid
		LEFT OUTER JOIN [List].[Lists] l on l.[Id]=lid.[ParentList_Id]
		WHERE lid.[ParentList_Id] = (SELECT top 1
									[ParentList_Id]
									FROM [List].[ListItemsDelta]
									WHERE [Sent] = 0)
			AND lid.[Sent] = 0
		ORDER BY lid.[Status]
END
GO