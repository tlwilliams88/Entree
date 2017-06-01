	   -- =============================================
       -- Author:			Brett Killins
       -- Create date:		5/18/2017
       -- Description:		Copies relevant contents from Favorites List Type in Generic List tables to their own tables
       -- =============================================

	INSERT -- create customer favorite lists
		INTO [BEK_Commerce_AppData].[List].[CustomListShares]
            ([CustomerNumber]
            ,[BranchId]
			,[ParentCustomListHeaderId])
        SELECT 
            ls.[CustomerId]
            ,ls.[BranchId]
			,clh.[Id]
        FROM 
            [BEK_Commerce_AppData].[List].[ListShares] as ls
			INNER JOIN [BEK_Commerce_AppData].[List].[Lists] l ON l.Id = ls.SharedList_Id
			INNER JOIN [BEK_Commerce_AppData].[List].[CustomListHeaders] clh ON clh.[CustomerNumber] = l.[CustomerId]
																				AND clh.[BranchId] = l.[BranchId]
																				AND clh.[Name] = l.[DisplayName]
