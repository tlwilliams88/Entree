	   -- =============================================
       -- Author:			Brett Killins
       -- Create date:		5/18/2017
       -- Description:		Copies relevant contents from reminder List Type in Generic List tables to their own tables
       -- =============================================

	INSERT -- create customer favorite lists
		INTO [List].[MandatoryItemsHeaders]
            ([CustomerNumber]
            ,[BranchId]
			 ,[ModifiedUtc]
			 ,[CreatedUtc])
        SELECT 
            l.[CustomerId]
            ,l.[BranchId]
			,GetUtcDate()
			,GetUtcDate()
        FROM 
            [List].[Lists] as l
		WHERE 
			l.[Type] = 9 AND
			NOT EXISTS (
				SELECT 
					'x'
					FROM [List].[MandatoryItemsHeaders]
					WHERE [CustomerNumber] = l.[CustomerId]
						AND [BranchId] = l.[BranchId]
						AND l.[Type] = 9)
		

    INSERT
		INTO [List].[MandatoryItemsDetails]
			([HeaderId]
			 ,[ItemNumber]
             ,[CatalogId]
             ,[Each]
			 ,[LineNumber]
			 ,[Active]
			 ,[ModifiedUtc]
			 ,[CreatedUtc])
		SELECT
 			fh.[Id]
			,li.[ItemNumber]
			,li.[CatalogId]
			,li.Each
			,li.[Position]
			,1
			,GetUtcDate()
			,GetUtcDate()
		FROM List.[ListItems] li
		INNER JOIN List.[Lists] l on l.Id = li.ParentList_Id
		INNER JOIN List.[MandatoryItemsHeaders] fh on fh.CustomerNumber = l.CustomerId and fh.BranchId = l.BranchId
		WHERE
			l.Type = 9 and
			NOT EXISTS (
				SELECT
					'x'
					FROM [List].[MandatoryItemsDetails]
					INNER JOIN List.[Lists] l on l.Id = li.ParentList_Id
					INNER JOIN List.[MandatoryItemsHeaders] fh on fh.CustomerNumber = l.CustomerId and fh.BranchId = l.BranchId
					WHERE [ItemNumber] = li.[ItemNumber]
						AND [Each] = li.[Each]
						AND [CatalogId] = li.[CatalogId])