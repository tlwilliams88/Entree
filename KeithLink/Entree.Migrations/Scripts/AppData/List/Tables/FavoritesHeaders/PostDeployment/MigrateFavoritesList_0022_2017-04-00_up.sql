	   -- =============================================
       -- Author:			Brett Killins
       -- Create date:		5/18/2017
       -- Description:		Copies relevant contents from Favorites List Type in Generic List tables to their own tables
       -- =============================================

	INSERT -- create customer favorite lists
		INTO [List].[FavoritesHeaders]
            ([CustomerNumber]
            ,[BranchId]
			,[UserId])
        SELECT 
            l.[CustomerId]
            ,l.[BranchId]
			,l.[UserId]
        FROM 
            [List].[Lists] as l
		WHERE 
			l.[Type] = 1 AND
			l.CustomerId IS NOT NULL AND
			NOT EXISTS (
				SELECT 
					'x'
					FROM [List].[FavoritesHeaders]
					WHERE [CustomerNumber] = l.[CustomerId]
						AND [BranchId] = l.[BranchId]
						AND [UserId] = l.[UserId]
						AND l.[Type] = 1)
		

    INSERT
		INTO [List].[FavoritesDetails]
			([HeaderId]
			 ,[ItemNumber]
			 ,[LineNumber]
             ,[CatalogId]
             ,[Each]
			 ,[Label]
			 ,[Active])
		SELECT
 			fh.[Id]
			,li.[ItemNumber]
			,li.[Position]
			,li.[CatalogId]
			,li.Each
			,li.Label
			,1
		FROM List.[ListItems] li
		INNER JOIN List.[Lists] l on l.Id = li.ParentList_Id
		INNER JOIN List.[FavoritesHeaders] fh on fh.CustomerNumber = l.CustomerId and fh.BranchId = l.BranchId and fh.[UserId] = l.[UserId]
		WHERE
			l.Type = 1 and
			NOT EXISTS (
				SELECT
					'x'
					FROM [List].[FavoritesDetails]
					INNER JOIN List.[Lists] l on l.Id = li.ParentList_Id
					INNER JOIN List.[FavoritesHeaders] fh on fh.CustomerNumber = l.CustomerId and fh.BranchId = l.BranchId and fh.[UserId] = l.[UserId]
					WHERE [ItemNumber] = li.[ItemNumber]
						AND [Each] = li.[Each]
						AND [CatalogId] = li.[CatalogId])