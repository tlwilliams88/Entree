	   -- =============================================
       -- Author:			Brett Killins
       -- Create date:		5/18/2017
       -- Description:		Copies relevant contents from reminder List Type in Generic List tables to their own tables
       -- =============================================

	INSERT -- create customer favorite lists
		INTO [BEK_Commerce_AppData].[List].[RemindersHeaders]
            ([CustomerNumber]
            ,[BranchId])
        SELECT 
            l.[CustomerId]
            ,l.[BranchId]
        FROM 
            [BEK_Commerce_AppData].[List].[Lists] as l
		WHERE 
			l.[Type] = 8 AND
			NOT EXISTS (
				SELECT 
					'x'
					FROM [BEK_Commerce_AppData].[List].[RemindersHeaders]
					WHERE [CustomerNumber] = l.[CustomerId]
						AND [BranchId] = l.[BranchId]
						AND l.[Type] = 8)
		

    INSERT
		INTO [BEK_Commerce_AppData].[List].[ReminderDetails]
			([ParentRemindersHeaderId]
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
		INNER JOIN List.[RemindersHeaders] fh on fh.CustomerNumber = l.CustomerId and fh.BranchId = l.BranchId
		WHERE
			l.Type = 8 and
			NOT EXISTS (
				SELECT
					'x'
					FROM [BEK_Commerce_AppData].[List].[ReminderDetails]
					INNER JOIN List.[Lists] l on l.Id = li.ParentList_Id
					INNER JOIN List.[RemindersHeaders] fh on fh.CustomerNumber = l.CustomerId and fh.BranchId = l.BranchId
					WHERE [ItemNumber] = li.[ItemNumber]
						AND [Each] = li.[Each]
						AND [CatalogId] = li.[CatalogId])