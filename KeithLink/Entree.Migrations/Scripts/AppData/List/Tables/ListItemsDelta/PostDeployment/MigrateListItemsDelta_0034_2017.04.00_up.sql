	   -- =============================================
       -- Author:			Brett Killins
       -- Create date:		5/18/2017
       -- Description:		Copies relevant contents from custom List Type in Generic List tables to their own tables
       -- =============================================

    INSERT -- insert shadow item into delta list
		INTO [List].[ListItemsDelta]
			([ItemNumber]
            ,[Each]
            ,[ParentList_Id]
            ,[CreatedUtc]
            ,[ModifiedUtc]
            ,[CatalogId]
            ,[Status]
			,[Sent])
        SELECT 
			LTRIM(RTRIM(bcd.ItemNumber))
			,CASE WHEN bcd.ForceEachOrCaseOnly = 'B' THEN 1 ELSE 0 END
			,l.Id
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
			INNER JOIN 
				List.[ContractHeaders] l
				ON l.CustomerNumber = ltrim(rtrim(cb.CustomerNumber)) and l.BranchId = ltrim(rtrim(cb.DivisionNumber))
			WHERE
				NOT EXISTS (
					SELECT
						'x'
						FROM [List].[ListItemsDelta] li
						WHERE li.ParentList_Id = l.Id 
							AND li.ItemNumber = LTRIM(RTRIM(bcd.ItemNumber))
							AND li.Each = CASE WHEN bcd.ForceEachOrCaseOnly = 'B' THEN 1 ELSE 0 END)
