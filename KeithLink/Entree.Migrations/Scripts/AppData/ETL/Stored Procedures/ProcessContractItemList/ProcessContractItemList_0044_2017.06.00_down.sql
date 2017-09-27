ALTER PROCEDURE [ETL].[ProcessContractItemList] 
AS
BEGIN
	   -- =============================================
       -- Author:			Brett Killins
       -- Create date:		1/31/2017
       -- Description:		Reconciles Entree Lists for Customer Contract items from Staging tables
       -- =============================================
       -- Newly added items can be identified by the CreatedUTC date
       -- Inactive items can be identified by a past ToDate, and are deleted after 2 weeks
    SET NOCOUNT ON

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
			,GETUTCDATE()
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

    INSERT -- insert shadow item into delta list
		INTO [List].[ListItemsDelta]
			([ItemNumber]
            ,[Each]
            ,[CustomerNumber]
            ,[BranchId]
            ,[CreatedUtc]
            ,[ModifiedUtc]
            ,[CatalogId]
            ,[Status])
        SELECT DISTINCT 
			LTRIM(RTRIM(bcd.ItemNumber))
			,CASE WHEN bcd.ForceEachOrCaseOnly = 'B' THEN 1 ELSE 0 END
			,ltrim(rtrim(cb.CustomerNumber))
            ,ltrim(rtrim(cb.DivisionNumber))
			,GETUTCDATE()
			,GETUTCDATE()
			,LTRIM(RTRIM(cb.DivisionNumber))
			,'Added'
			FROM 
				[ETL].[Staging_BidContractDetail] bcd
			INNER JOIN 
				[ETL].[Staging_CustomerBid] cb
				ON cb.BidNumber=bcd.BidNumber AND cb.DivisionNumber = bcd.DivisionNumber
			WHERE
				NOT EXISTS (
					SELECT
						'x'
						FROM [List].[ListItemsDelta] li
						WHERE li.[CustomerNumber] = ltrim(rtrim(cb.CustomerNumber))
                              AND li.[BranchId] = ltrim(rtrim(cb.DivisionNumber))
							  AND li.ItemNumber = LTRIM(RTRIM(bcd.ItemNumber))
							  AND li.Each = CASE WHEN bcd.ForceEachOrCaseOnly = 'B' THEN 1 ELSE 0 END)

	UPDATE
		[List].[ContractDetails]
		SET 
			[LineNumber] = CAST(bcd.BidLineNumber as int) 
			, Category = LTRIM(RTRIM(bcd.CategoryDescription))
			, ToDate = DATEADD(day, 1, GETDATE())
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

    INSERT -- insert shadow item into delta list
		INTO [List].[ListItemsDelta]
			([ItemNumber]
            ,[Each]
            ,[CustomerNumber]
            ,[BranchId]
            ,[CreatedUtc]
            ,[ModifiedUtc]
            ,[CatalogId]
            ,[Status])
        SELECT DISTINCT 
			li.ItemNumber
			,li.Each
			,ltrim(rtrim(cb.CustomerNumber))
            ,ltrim(rtrim(cb.DivisionNumber))
			,GETUTCDATE()
			,GETUTCDATE()
			,l.BranchId
			,'Deleted'
		FROM [List].[ContractDetails] li
			INNER JOIN [List].[ContractHeaders] l
			ON li.HeaderId = l.Id
            INNER JOIN [ETL].[Staging_CustomerBid] cb
                ON ltrim(rtrim(cb.CustomerNumber))=l.CustomerNumber AND ltrim(rtrim(cb.DivisionNumber)) = l.BranchId
	WHERE 
		NOT EXISTS (
			SELECT
				'x'
				FROM [ETL].[Staging_BidContractDetail] bcd
				INNER JOIN 
					[ETL].[Staging_CustomerBid] cb
					ON cb.BidNumber=bcd.BidNumber AND cb.DivisionNumber = bcd.DivisionNumber
				WHERE
					ltrim(rtrim(ItemNumber)) = li.ItemNumber
					AND CASE WHEN bcd.ForceEachOrCaseOnly = 'B' THEN 1 ELSE 0 END = li.Each
					AND ltrim(rtrim(cb.CustomerNumber)) = l.CustomerNumber 
					AND ltrim(rtrim(cb.DivisionNumber)) = l.BranchId
			)
		AND NOT EXISTS (
			SELECT
				'x'
				FROM [List].[ListItemsDelta] lid
				WHERE
					lid.ItemNumber = li.ItemNumber
					AND lid.Each = li.Each
					AND lid.[Status] = 'Deleted' 
					AND ltrim(rtrim(cb.CustomerNumber)) = lid.CustomerNumber 
					AND ltrim(rtrim(cb.DivisionNumber)) = lid.BranchId
			)


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
			AND ToDate < DATEADD(day, -13, GETDATE())
END