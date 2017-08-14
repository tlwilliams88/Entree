	   -- =============================================
       -- Author:			Brett Killins
       -- Create date:		5/18/2017
       -- Description:		Copies relevant contents from custom List Type in Generic List tables to their own tables
       -- =============================================

    INSERT 
		INTO [List].[ListItemsDelta]
			([ItemNumber]
            ,[Each]
            ,[CustomerNumber]
            ,[BranchId]
            ,[CreatedUtc]
            ,[ModifiedUtc]
            ,[CatalogId]
            ,[Status]
			,[Sent])
        SELECT 
			LTRIM(RTRIM(bcd.ItemNumber))
			,CASE WHEN bcd.ForceEachOrCaseOnly = 'B' THEN 1 ELSE 0 END
			,ltrim(rtrim(cb.CustomerNumber))
            ,ltrim(rtrim(cb.DivisionNumber))
			,GETUTCDATE()
			,GETUTCDATE()
			,LTRIM(RTRIM(cb.DivisionNumber))
			,'Migrated'
			,1
			FROM 
				[ETL].[Staging_BidContractDetail] bcd
			INNER JOIN 
				[ETL].[Staging_CustomerBid] cb
				ON cb.BidNumber=bcd.BidNumber AND cb.DivisionNumber = bcd.DivisionNumber
