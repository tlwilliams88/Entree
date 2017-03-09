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
		INTO [BEK_Commerce_AppData].[List].[Lists]
            ([DisplayName]
            ,[Type]
            ,[CustomerId]
            ,[BranchId]
            ,[ReadOnly]
            ,[CreatedUtc]
            ,[ModifiedUtc])
        SELECT DISTINCT
            'Contract - ' + LTRIM(RTRIM(cb.[BidNumber]))
            ,2
            ,LTRIM(RTRIM(cb.[CustomerNumber]))
            ,LTRIM(RTRIM(cb.[DivisionNumber]))
            ,1
            ,GETUTCDATE()
            ,GETUTCDATE()
        FROM 
            [BEK_Commerce_AppData].[ETL].[Staging_CustomerBid] as cb
		WHERE 
			NOT EXISTS (
				SELECT 
					'x'
					FROM [BEK_Commerce_AppData].[List].[Lists]
					WHERE [CustomerId] = LTRIM(RTRIM(cb.[CustomerNumber]))
						AND [BranchId] = LTRIM(RTRIM(cb.[DivisionNumber]))
						AND [Type] = 2)

    INSERT -- insert new items into all customer lists
		INTO [BEK_Commerce_AppData].[List].[ListItems]
			([ItemNumber]
            ,[Par]
            ,[CreatedUtc]
            ,[ParentList_Id]
            ,[ModifiedUtc]
            ,[Category]
            ,[Position]
            ,[Each]
            ,[CatalogId]
            ,[ToDate])
        SELECT
			LTRIM(RTRIM(bcd.ItemNumber))
			,0.00
			,GETUTCDATE()
			,l.Id
			,GETUTCDATE()
			,LTRIM(RTRIM(bcd.CategoryDescription))
			,CAST(bcd.BidLineNumber as int)
			,CASE WHEN bcd.ForceEachOrCaseOnly = 'B' THEN 1 ELSE 0 END
			,LTRIM(RTRIM(cb.DivisionNumber))
			,DATEADD(day, 1, GETDATE())
			FROM 
				[BEK_Commerce_AppData].[ETL].[Staging_BidContractDetail] bcd
			INNER JOIN 
				[BEK_Commerce_AppData].[ETL].[Staging_CustomerBid] cb
				ON cb.BidNumber=bcd.BidNumber AND cb.DivisionNumber = bcd.DivisionNumber
			INNER JOIN 
				[BEK_Commerce_AppData].List.Lists l
				ON l.CustomerId = ltrim(rtrim(cb.CustomerNumber)) and l.BranchId = ltrim(rtrim(cb.DivisionNumber)) and l.Type = 2
			WHERE
				NOT EXISTS (
					SELECT
						'x'
						FROM [BEK_Commerce_AppData].[List].[ListItems] li
						WHERE li.ParentList_Id = l.Id 
							AND li.ItemNumber = LTRIM(RTRIM(bcd.ItemNumber))
							AND li.Each = CASE WHEN bcd.ForceEachOrCaseOnly = 'B' THEN 1 ELSE 0 END)

	UPDATE
		[BEK_Commerce_AppData].[List].[ListItems]
		SET 
			Position = CAST(bcd.BidLineNumber as int) 
			, Category = LTRIM(RTRIM(bcd.CategoryDescription))
			, ToDate = DATEADD(day, 3, GETDATE())
		FROM 
			[BEK_Commerce_AppData].[List].[ListItems] li
		INNER JOIN 
			[BEK_Commerce_AppData].List.Lists l
			ON l.Id = li.ParentList_Id and l.Type = 2
		INNER JOIN
			[BEK_Commerce_AppData].[ETL].[Staging_BidContractDetail] bcd
			ON ltrim(rtrim(bcd.ItemNumber)) = li.ItemNumber
			AND CASE WHEN ltrim(rtrim(bcd.ForceEachOrCaseOnly)) = 'B' THEN 1 ELSE 0 END = li.Each
		INNER JOIN 
			[BEK_Commerce_AppData].[ETL].[Staging_CustomerBid] cb
			ON cb.BidNumber=bcd.BidNumber 
				AND cb.DivisionNumber = bcd.DivisionNumber
				AND l.CustomerId = ltrim(rtrim(cb.CustomerNumber)) and l.BranchId = ltrim(rtrim(cb.DivisionNumber))

	UPDATE
		[BEK_Commerce_AppData].[List].[ListItems]
		SET 
			ToDate = DATEADD(day, -14, GETDATE())
		FROM 
			[BEK_Commerce_AppData].[List].[ListItems] li
		INNER JOIN 
			[BEK_Commerce_AppData].List.Lists l
			ON l.Id = li.ParentList_Id and l.Type = 2
		LEFT OUTER JOIN
			[BEK_Commerce_AppData].[ETL].[Staging_BidContractDetail] bcd
			ON ltrim(rtrim(bcd.ItemNumber)) = li.ItemNumber
			AND CASE WHEN ltrim(rtrim(bcd.ForceEachOrCaseOnly)) = 'B' THEN 1 ELSE 0 END = li.Each
		LEFT OUTER JOIN 
			[BEK_Commerce_AppData].[ETL].[Staging_CustomerBid] cb
			ON cb.BidNumber=bcd.BidNumber 
				AND cb.DivisionNumber = bcd.DivisionNumber
				AND l.CustomerId = ltrim(rtrim(cb.CustomerNumber)) and l.BranchId = ltrim(rtrim(cb.DivisionNumber))
		WHERE bcd.ItemNumber is null and li.ToDate is null

	DELETE TOP (50000)
		FROM [BEK_Commerce_AppData].[List].[ListItems]
		FROM [BEK_Commerce_AppData].[List].[ListItems] li
			INNER JOIN [BEK_Commerce_AppData].[List].[Lists] l
			ON li.ParentList_Id = l.Id
			AND l.[Type] = 2
	WHERE 
		NOT EXISTS (
			SELECT
				'x'
				FROM [BEK_Commerce_AppData].[ETL].[Staging_BidContractDetail] bcd
				INNER JOIN 
					[BEK_Commerce_AppData].[ETL].[Staging_CustomerBid] cb
					ON cb.BidNumber=bcd.BidNumber AND cb.DivisionNumber = bcd.DivisionNumber
				INNER JOIN 
					[BEK_Commerce_AppData].List.Lists l
					ON l.CustomerId = ltrim(rtrim(cb.CustomerNumber)) and l.BranchId = ltrim(rtrim(cb.DivisionNumber)) and l.Type = 2
				WHERE
					ltrim(rtrim(ItemNumber)) = li.ItemNumber
					AND CASE WHEN bcd.ForceEachOrCaseOnly = 'B' THEN 1 ELSE 0 END = li.Each
					AND ltrim(rtrim(cb.CustomerNumber)) = l.CustomerId 
					and ltrim(rtrim(cb.DivisionNumber)) = l.BranchId
			)
			AND ToDate < DATEADD(day, -13, GETDATE())

END