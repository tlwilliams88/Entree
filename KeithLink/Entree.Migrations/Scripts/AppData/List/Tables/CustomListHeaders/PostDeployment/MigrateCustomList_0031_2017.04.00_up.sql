	   -- =============================================
       -- Author:			Brett Killins
       -- Create date:		5/18/2017
       -- Description:		Copies relevant contents from custom List Type in Generic List tables to their own tables
       -- =============================================

	INSERT -- create customer custom lists
		INTO [List].[CustomListHeaders]
            ([CustomerNumber]
            ,[BranchId]
            ,[Name]
			,[Active]
			,[ModifiedUtc]
			,[CreatedUtc])
        SELECT 
            l.[CustomerId]
            ,l.[BranchId]
            ,case when l.[DisplayName] is null then 'Custom' else left(l.[DisplayName],100) end
			,1
			,GetUtcDate()
			,GetUtcDate()
        FROM 
            [List].[Lists] as l
		WHERE 
			l.[Type] = 0 AND
			NOT EXISTS (
				SELECT 
					'x'
					FROM [List].[CustomListHeaders]
					WHERE [CustomerNumber] = l.[CustomerId]
						AND [BranchId] = l.[BranchId]
						AND l.[Type] = 0)
		

    INSERT
		INTO [List].[CustomListDetails]
			([HeaderId]
			 ,[ItemNumber]
			 ,[LineNumber]
             ,[CatalogId]
             ,[Each]
			 ,[Par]
			 ,[Label]
			 ,[CustomInventoryItemId]
			 ,[Active]
			,[ModifiedUtc]
			,[CreatedUtc])
		SELECT
 			fh.[Id]
			,li.[ItemNumber]
			,li.[Position]
			,li.[CatalogId]
			,li.Each
			,li.Par
			,li.Label
			,li.CustomInventoryItemId
			,1
			,GetUtcDate()
			,GetUtcDate()
		FROM List.[ListItems] li
		INNER JOIN List.[Lists] l on l.Id = li.ParentList_Id
		INNER JOIN List.[CustomListHeaders] fh on fh.CustomerNumber = l.CustomerId and fh.BranchId = l.BranchId and fh.[Name] = l.[DisplayName]
		WHERE
			l.Type = 0 and
			NOT EXISTS (
				SELECT
					'x'
					FROM [List].[CustomListDetails]
					INNER JOIN List.[Lists] l on l.Id = li.ParentList_Id
					INNER JOIN List.[InventoryValuationListHeaders] fh on fh.CustomerNumber = l.CustomerId and fh.BranchId = l.BranchId
					WHERE [ItemNumber] = li.[ItemNumber]
						AND [Each] = li.[Each]
						AND [CatalogId] = li.[CatalogId])