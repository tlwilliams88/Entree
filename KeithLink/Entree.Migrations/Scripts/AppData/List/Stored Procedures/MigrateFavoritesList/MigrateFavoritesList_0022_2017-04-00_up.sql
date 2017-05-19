CREATE PROCEDURE [List].[MigrateFavoritesList] 
AS
BEGIN
	   -- =============================================
       -- Author:			Brett Killins
       -- Create date:		5/18/2017
       -- Description:		Copies relevant contents from Favorites List Type in Generic List tables to their own tables
       -- =============================================

	INSERT -- create customer favorite lists
		INTO [BEK_Commerce_AppData].[List].[FavoritesHeader]
            ([CustomerNumber]
            ,[BranchId]
			,[UserId]
            ,[Name])
        SELECT 
            l.[CustomerId]
            ,l.[BranchId]
			,l.[UserId]
            ,'Favorites'
        FROM 
            [BEK_Commerce_AppData].[List].[Lists] as l
		WHERE 
			l.[Type] = 1 AND
			NOT EXISTS (
				SELECT 
					'x'
					FROM [BEK_Commerce_AppData].[List].[FavoritesHeader]
					WHERE [CustomerNumber] = l.[CustomerId]
						AND [BranchId] = l.[BranchId]
						AND [UserId] = l.[UserId]
						AND l.[Type] = 1)
		

    INSERT
		INTO [BEK_Commerce_AppData].[List].[FavoritesDetail]
			([ParentFavoritesHeaderId]
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
		INNER JOIN List.[FavoritesHeader] fh on fh.CustomerNumber = l.CustomerId and fh.BranchId = l.BranchId and fh.[UserId] = l.[UserId]
		WHERE
			l.Type = 1 and
			NOT EXISTS (
				SELECT
					'x'
					FROM [BEK_Commerce_AppData].[List].[FavoritesDetail]
					INNER JOIN List.[Lists] l on l.Id = li.ParentList_Id
					INNER JOIN List.[FavoritesHeader] fh on fh.CustomerNumber = l.CustomerId and fh.BranchId = l.BranchId and fh.[UserId] = l.[UserId]
					WHERE [ItemNumber] = li.[ItemNumber]
						AND [Each] = li.[Each]
						AND [CatalogId] = li.[CatalogId])

END