

GO
-- Clean up bad data created from previous version
DELETE FROM [List].[ListItems]
	WHERE Id IN (SELECT l.Id FROM [List].[ListItems] l
			LEFT JOIN [List].[CustomInventoryItems] c ON c.Id = l.CustomInventoryItemId
			WHERE l.CustomInventoryItemId IS NOT NULL
			AND   c.Id IS NULL)