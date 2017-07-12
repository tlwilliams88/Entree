	INSERT -- create customer contract lists from staging customerbids if they don't exist 
		INTO [List].[HistoryHeaders]
            ([CustomerNumber]
            ,[BranchId]
            ,[CreatedUtc]
            ,[ModifiedUtc])
        SELECT DISTINCT
            ws.[CustomerNumber],
            ws.[DivisionNumber]
            ,GETUTCDATE()
            ,GETUTCDATE()
        FROM 
            [ETL].[Staging_WorksheetItems] as ws

    INSERT -- insert new items into all customer lists
		INTO [List].[HistoryDetails]
             ([HeaderId]
              ,[LineNumber]
              ,[ItemNumber]
              ,[Each]
              ,[CatalogId]
              ,[CreatedUtc]
              ,[ModifiedUtc])
        SELECT
		     h.[Id]
		     ,0
			 ,LTRIM(RTRIM(ws.ItemNumber))
			 ,CASE WHEN ws.BrokenCaseCode = 'Y' THEN 1 ELSE 0 END
			 ,LTRIM(RTRIM(ws.DivisionNumber))
			 ,GETUTCDATE()
			 ,GETUTCDATE()
			FROM 
				[ETL].[Staging_WorksheetItems] ws
			INNER JOIN 
				[List].[HistoryHeaders] h
				ON h.[CustomerNumber] = ltrim(rtrim(ws.CustomerNumber)) and h.BranchId = ltrim(rtrim(ws.DivisionNumber))
GO
