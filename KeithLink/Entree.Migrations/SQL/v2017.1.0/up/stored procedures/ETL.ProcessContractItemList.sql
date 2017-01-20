ALTER PROCEDURE [ETL].[ProcessContractItemList] 
AS
BEGIN
	-- =============================================
	-- Author:		Brett Killins
	-- Create date: 1/20/2017
	-- Description:	Reconciles Entree Lists for Customer Contract items from Staging tables
	-- =============================================
	-- Newly added items can be identified by the CreatedUTC date
	-- Inactive items can be identified by a past ToDate, and are deleted after 2 weeks
	SET NOCOUNT ON

	MERGE  -- create customer contract lists from staging customerbids
		INTO [BEK_Commerce_AppData].[List].[Lists] AS target
		USING (
			SELECT DISTINCT
				LTRIM(RTRIM([CustomerNumber])) AS CustomerId
				,LTRIM(RTRIM([DivisionNumber])) AS BranchId
				,LTRIM(RTRIM([BidNumber])) AS BidNumber
				,1 AS SETREADONLY
				,2 AS LISTTYPE
			FROM 
				[BEK_Commerce_AppData].[ETL].[Staging_CustomerBid] 
				) AS source 
			ON (target.[CustomerId] = source.[CustomerId]) 
				AND (target.[BranchId] = source.[BranchId])
				AND (target.[Type] = source.LISTTYPE)
		WHEN NOT MATCHED
			THEN
				INSERT
					([DisplayName]
					,[Type]
					,[CustomerId]
					,[BranchId]
					,[ReadOnly]
					,[CreatedUtc]
					,[ModifiedUtc])
				VALUES
					('Contract - ' + source.BidNumber
					,source.LISTTYPE
					,source.CustomerId
					,source.BranchId
					,source.SETREADONLY
					,GETUTCDATE()
					,GETUTCDATE())
		;

	MERGE -- insert/delete the actual contract lists
		INTO [BEK_Commerce_AppData].[List].[ListItems] AS target
		USING (
			SELECT  
				LTRIM(RTRIM(bcd.BidNumber)) AS BidNumber
				,LTRIM(RTRIM(cb.CustomerNumber)) AS CustomerId
				,LTRIM(RTRIM(cb.DivisionNumber)) AS BranchId
				,l.Id AS ContractListId
				,LTRIM(RTRIM(bcd.ItemNumber)) AS ItemNumber
				,LTRIM(RTRIM(bcd.CategoryDescription)) AS CCategory
				,CAST(bcd.BidLineNumber as int) AS CLineNumber
				,CASE WHEN bcd.ForceEachOrCaseOnly = 'B' THEN 1 ELSE 0 END AS Each
				,DATEADD(day, 1, GETDATE()) AS Tomorrow
			FROM 
				[BEK_Commerce_AppData].[ETL].[Staging_BidContractDetail] bcd
			INNER JOIN 
				[BEK_Commerce_AppData].[ETL].[Staging_CustomerBid] cb
				ON cb.BidNumber=bcd.BidNumber AND cb.DivisionNumber = bcd.DivisionNumber
			INNER JOIN 
				[BEK_Commerce_AppData].List.Lists l
				ON l.CustomerId = ltrim(rtrim(cb.CustomerNumber)) and l.BranchId = ltrim(rtrim(cb.DivisionNumber)) and l.Type = 2
				) AS source 
			ON (target.[ParentList_Id] = source.ContractListId)
				AND (target.ItemNumber = source.ItemNumber)
		WHEN NOT MATCHED BY TARGET
			THEN -- insert a new contract list item
				INSERT
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
				VALUES
					(source.ItemNumber
					,0.00
					,GETUTCDATE()
					,source.ContractListId
					,GETUTCDATE()
					,source.CCategory
					,source.CLineNumber
					,source.Each
					,source.BranchId
					,source.Tomorrow)
		WHEN NOT MATCHED BY SOURCE AND target.ToDate < DATEADD(day, -13, GETDATE())
			THEN -- DELETE Item
				DELETE
		WHEN MATCHED
			THEN
				UPDATE SET target.Position = source.CLineNumber, target.Category = source.CCategory, target.ToDate = source.Tomorrow
		;
		

END
