﻿	   -- =============================================
       -- Author:			Brett Killins
       -- Create date:		5/18/2017
       -- Description:		Copies relevant contents from reminder List Type in Generic List tables to their own tables
       -- =============================================

	INSERT -- create customer favorite lists
		INTO [List].[InventoryValuationListHeaders]
            ([CustomerNumber]
            ,[BranchId]
            ,[Name]
			,[Active]
			 ,[ModifiedUtc]
			 ,[CreatedUtc])
        SELECT 
            l.[CustomerId]
            ,l.[BranchId]
            ,left(l.[DisplayName],150)
			,1
			,GetUtcDate()
			,GetUtcDate()
        FROM 
            [List].[Lists] as l
		WHERE 
			l.[Type] = 11 AND
			NOT EXISTS (
				SELECT 
					'x'
					FROM [List].[InventoryValuationListHeaders]
					WHERE [CustomerNumber] = l.[CustomerId]
						AND [BranchId] = l.[BranchId]
						AND l.[Type] = 11)
		

    INSERT
		INTO [List].[InventoryValuationListDetails]
			([HeaderId]
			 ,[ItemNumber]
			 ,[LineNumber]
             ,[CatalogId]
             ,[Each]
			 ,[Quantity]
			 ,[ModifiedUtc]
			 ,[CreatedUtc])
		SELECT
 			fh.[Id]
			,left(li.[ItemNumber],6)
			,li.[Position]
			,li.[CatalogId]
			,li.Each
			,li.Quantity
			,GetUtcDate()
			,GetUtcDate()
		FROM List.[ListItems] li
		INNER JOIN List.[Lists] l on l.Id = li.ParentList_Id
		INNER JOIN List.[InventoryValuationListHeaders] fh on fh.CustomerNumber = l.CustomerId and fh.BranchId = l.BranchId
		WHERE
			l.Type = 11 and
			NOT EXISTS (
				SELECT
					'x'
					FROM [List].[InventoryValuationListDetails]
					INNER JOIN List.[Lists] l on l.Id = li.ParentList_Id
					INNER JOIN List.[InventoryValuationListHeaders] fh on fh.CustomerNumber = l.CustomerId and fh.BranchId = l.BranchId
					WHERE [ItemNumber] = li.[ItemNumber]
						AND [Each] = li.[Each]
						AND [CatalogId] = li.[CatalogId])