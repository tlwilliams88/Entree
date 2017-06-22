﻿	   -- =============================================
       -- Author:			Brett Killins
       -- Create date:		5/18/2017
       -- Description:		Copies relevant contents from custom List Type in Generic List tables to their own tables
       -- =============================================

	INSERT -- create customer custom lists
		INTO [List].[CustomListHeaders]
            ([CustomerNumber]
            ,[BranchId]
            ,[Name])
        SELECT 
            l.[CustomerId]
            ,l.[BranchId]
            ,l.[DisplayName]
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
			([ParentCustomListHeaderId]
			 ,[ItemNumber]
             ,[CatalogId]
             ,[Each]
			 ,[Par]
			 ,[Label]
			 ,[CustomInventoryItemId])
		SELECT
 			fh.[Id]
			,li.[ItemNumber]
			,li.[CatalogId]
			,li.Each
			,li.Par
			,li.Label
			,li.CustomInventoryItemId
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