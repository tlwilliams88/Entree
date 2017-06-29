	   -- =============================================
       -- Author:			Brett Killins
       -- Create date:		5/18/2017
       -- Description:		Copies relevant contents from Favorites List Type in Generic List tables to their own tables
       -- =============================================

	INSERT -- create customer favorite lists
		INTO [List].[CustomListShares]
            ([CustomerNumber]
            ,[BranchId]
			,[HeaderId])
        SELECT 
            ls.[CustomerId]
            ,ls.[BranchId]
			,clh.[Id]
        FROM 
            [List].[ListShares] as ls
			INNER JOIN [List].[Lists] l ON l.Id = ls.SharedList_Id
			INNER JOIN [List].[CustomListHeaders] clh ON clh.[CustomerNumber] = l.[CustomerId]
																				AND clh.[BranchId] = l.[BranchId]
																				AND clh.[Name] = l.[DisplayName]
