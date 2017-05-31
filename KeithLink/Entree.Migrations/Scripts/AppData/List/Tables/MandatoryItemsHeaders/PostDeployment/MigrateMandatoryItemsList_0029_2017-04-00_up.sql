	   -- =============================================
       -- Author:			Brett Killins
       -- Create date:		5/18/2017
       -- Description:		Copies relevant contents from reminder List Type in Generic List tables to their own tables
       -- =============================================

	INSERT -- create customer favorite lists
		INTO [BEK_Commerce_AppData].[List].[MandatoryItemsHeaders]
            ([CustomerNumber]
            ,[BranchId]
            ,[Name])
        SELECT 
            l.[CustomerId]
            ,l.[BranchId]
            ,'Mandatory Items'
        FROM 
            [BEK_Commerce_AppData].[List].[Lists] as l
		WHERE 
			l.[Type] = 9 AND
			NOT EXISTS (
				SELECT 
					'x'
					FROM [BEK_Commerce_AppData].[List].[MandatoryItemsHeaders]
					WHERE [CustomerNumber] = l.[CustomerId]
						AND [BranchId] = l.[BranchId]
						AND l.[Type] = 9)
		

    INSERT
		INTO [BEK_Commerce_AppData].[List].[MandatoryItemsDetails]
			([ParentMandatoryItemsHeaderId]
			 ,[ItemNumber]
             ,[CatalogId]
             ,[Each])
		SELECT
 			fh.[Id]
			,li.[ItemNumber]
			,li.[CatalogId]
			,li.Each
		FROM List.[ListItems] li
		INNER JOIN List.[Lists] l on l.Id = li.ParentList_Id
		INNER JOIN List.[MandatoryItemsHeaders] fh on fh.CustomerNumber = l.CustomerId and fh.BranchId = l.BranchId
		WHERE
			l.Type = 9 and
			NOT EXISTS (
				SELECT
					'x'
					FROM [BEK_Commerce_AppData].[List].[MandatoryItemsDetails]
					INNER JOIN List.[Lists] l on l.Id = li.ParentList_Id
					INNER JOIN List.[MandatoryItemsHeaders] fh on fh.CustomerNumber = l.CustomerId and fh.BranchId = l.BranchId
					WHERE [ItemNumber] = li.[ItemNumber]
						AND [Each] = li.[Each]
						AND [CatalogId] = li.[CatalogId])