﻿-- =============================================
-- Author:			Matt Joiner
-- Create date:		05/22/2017
-- Description:	    Migrates the contract header and detail data post creation scripts to the new tables	
-- =============================================
	   
-- Migrate Header
	INSERT -- create customer contract lists from staging customerbids if they don't exist 
		INTO [List].[ContractHeaders]
            ([ContractId]
            ,[CustomerNumber]
            ,[BranchId]
            ,[CreatedUtc]
            ,[ModifiedUtc])
        SELECT DISTINCT
             LTRIM(RTRIM(cb.[BidNumber]))
            ,LTRIM(RTRIM(cb.[CustomerNumber]))
            ,LTRIM(RTRIM(cb.[DivisionNumber]))
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
			,'5/1/2017'
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

       UPDATE
           [List].[ContractHeaders]
       SET 
           ContractId = ltrim(rtrim(cb.BidNumber))
       FROM 
           [List].[ContractHeaders] l
       INNER JOIN 
           [ETL].[Staging_CustomerBid] cb
           ON  l.BranchId = ltrim(rtrim(cb.DivisionNumber))
                   AND l.[CustomerNumber] = ltrim(rtrim(cb.CustomerNumber))
       WHERE
           l.ContractId <> ltrim(rtrim(cb.BidNumber))

	UPDATE
		[List].[ContractDetails]
		SET 
			[LineNumber] = CAST(bcd.BidLineNumber as int) 
			, Category = LTRIM(RTRIM(bcd.CategoryDescription))
			, ToDate = DATEADD(day, 3, GETDATE())
		FROM 
			[List].[ContractDetails] li
		INNER JOIN 
			List.[ContractHeaders] l
			ON l.Id = li.[HeaderId]
		INNER JOIN
			[ETL].[Staging_BidContractDetail] bcd
			ON ltrim(rtrim(bcd.ItemNumber)) = li.ItemNumber
			AND CASE WHEN ltrim(rtrim(bcd.ForceEachOrCaseOnly)) = 'B' THEN 1 ELSE 0 END = li.Each
		INNER JOIN 
			[ETL].[Staging_CustomerBid] cb
			ON cb.BidNumber=bcd.BidNumber 
				AND cb.DivisionNumber = bcd.DivisionNumber
				AND l.[CustomerNumber] = ltrim(rtrim(cb.CustomerNumber)) and l.BranchId = ltrim(rtrim(cb.DivisionNumber))

	UPDATE
		[List].[ContractDetails]
		SET 
			ToDate = DATEADD(day, -14, GETDATE())
		FROM 
			[List].[ContractDetails] li
		INNER JOIN 
			List.[ContractHeaders] l
			ON l.Id = li.[HeaderId]
		LEFT OUTER JOIN
			[ETL].[Staging_BidContractDetail] bcd
			ON ltrim(rtrim(bcd.ItemNumber)) = li.ItemNumber
			AND CASE WHEN ltrim(rtrim(bcd.ForceEachOrCaseOnly)) = 'B' THEN 1 ELSE 0 END = li.Each
		LEFT OUTER JOIN 
			[ETL].[Staging_CustomerBid] cb
			ON cb.BidNumber=bcd.BidNumber 
				AND cb.DivisionNumber = bcd.DivisionNumber
				AND l.[CustomerNumber] = ltrim(rtrim(cb.CustomerNumber)) and l.BranchId = ltrim(rtrim(cb.DivisionNumber))
		WHERE bcd.ItemNumber is null and li.ToDate is null

	DELETE TOP (50000)
		FROM [List].[ContractDetails]
		FROM [List].[ContractDetails] li
			INNER JOIN [List].[ContractHeaders] l
			ON li.[HeaderId] = l.Id
	WHERE 
		NOT EXISTS (
			SELECT
				'x'
				FROM [ETL].[Staging_BidContractDetail] bcd
				INNER JOIN 
					[ETL].[Staging_CustomerBid] cb
					ON cb.BidNumber=bcd.BidNumber AND cb.DivisionNumber = bcd.DivisionNumber
				INNER JOIN 
					List.[ContractHeaders] l
					ON l.[CustomerNumber] = ltrim(rtrim(cb.CustomerNumber)) and l.BranchId = ltrim(rtrim(cb.DivisionNumber))
				WHERE
					ltrim(rtrim(ItemNumber)) = li.ItemNumber
					AND CASE WHEN bcd.ForceEachOrCaseOnly = 'B' THEN 1 ELSE 0 END = li.Each
					AND ltrim(rtrim(cb.CustomerNumber)) = l.[CustomerNumber] 
					and ltrim(rtrim(cb.DivisionNumber)) = l.BranchId
			)
			--AND ToDate < DATEADD(day, -13, GETDATE())
			--remove delayed delete
