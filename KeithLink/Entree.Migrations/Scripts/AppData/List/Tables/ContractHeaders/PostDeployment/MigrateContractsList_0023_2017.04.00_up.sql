-- =============================================
-- Author:			Matt Joiner
-- Create date:		05/22/2017
-- Description:	    Migrates the contract header and detail data post creation scripts to the new tables	
-- =============================================
	   
	INSERT -- create customer contract lists from staging customerbids if they don't exist 
		INTO [List].[ContractHeaders]
            ([CustomerNumber]
            ,[BranchId]
            ,[ContractId]
            ,[CreatedUtc]
            ,[ModifiedUtc])
        SELECT DISTINCT
             LTRIM(RTRIM(cb.[CustomerNumber]))
            ,LTRIM(RTRIM(cb.[DivisionNumber]))
            ,LTRIM(RTRIM(cb.[BidNumber]))
            ,GETUTCDATE()
            ,GETUTCDATE()
        FROM 
            [ETL].[Staging_CustomerBid] as cb
		WHERE 
			NOT EXISTS (
				SELECT 
					'x'
					FROM [List].[ContractHeaders]
					WHERE [CustomerNumber] = LTRIM(RTRIM(cb.[CustomerNumber]))
						AND [BranchId] = LTRIM(RTRIM(cb.[DivisionNumber])))

    INSERT -- insert new items into all customer lists
		INTO [List].[ContractDetails]
			([ItemNumber]
            ,[CreatedUtc]
            ,[HeaderId]
            ,[ModifiedUtc]
            ,[Category]
            ,[LineNumber]
            ,[Each]
            ,[CatalogId]
            ,[ToDate])
        SELECT
			LTRIM(RTRIM(bcd.ItemNumber))
			,dateadd(day,-15, cast(getdate() as date))
			,l.Id
			,GETUTCDATE()
			,LTRIM(RTRIM(bcd.CategoryDescription))
			,CAST(bcd.BidLineNumber as int)
			,CASE WHEN bcd.ForceEachOrCaseOnly = 'B' THEN 1 ELSE 0 END
			,LTRIM(RTRIM(cb.DivisionNumber))
			,DATEADD(day, 1, GETDATE())
			FROM 
				[ETL].[Staging_BidContractDetail] bcd
			INNER JOIN 
				[ETL].[Staging_CustomerBid] cb
				ON cb.BidNumber=bcd.BidNumber AND cb.DivisionNumber = bcd.DivisionNumber
			INNER JOIN 
				List.[ContractHeaders] l
				ON l.[CustomerNumber] = ltrim(rtrim(cb.CustomerNumber)) and l.BranchId = ltrim(rtrim(cb.DivisionNumber))
			WHERE
				NOT EXISTS (
					SELECT
						'x'
						FROM [List].[ContractDetails] li
						WHERE li.[HeaderId] = l.Id 
							AND li.ItemNumber = LTRIM(RTRIM(bcd.ItemNumber))
							AND li.Each = CASE WHEN bcd.ForceEachOrCaseOnly = 'B' THEN 1 ELSE 0 END)
