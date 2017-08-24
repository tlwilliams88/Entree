  INSERT
        INTO [List].[NotesHeaders]
            ([CustomerNumber]
            ,[BranchId]
             ,[ModifiedUtc]
             ,[CreatedUtc])
        SELECT
            l.[CustomerId]
            ,l.[BranchId]
            ,GetUtcDate()
            ,GetUtcDate()
        FROM
            [List].[Lists] as l
        WHERE
            l.[Type] = 4 AND
            l.[CustomerId] IS NOT NULL AND
            l.[BranchId] IS NOT NULL AND
            NOT EXISTS (
                SELECT
                    'x'
                    FROM [List].[NotesHeaders]
                    WHERE [CustomerNumber] = l.[CustomerId]
                        AND [BranchId] = l.[BranchId]
                        AND l.[Type] = 4)
 
    INSERT
        INTO [List].[NotesDetails]
            ([HeaderId]
             ,[ItemNumber]
             ,[CatalogId]
             ,[Each]
             ,[Note]
             ,[ModifiedUtc]
             ,[CreatedUtc])
        SELECT DISTINCT
            fh.[Id]
            ,li.[ItemNumber]
            ,li.[CatalogId]
            ,li.Each
            ,li.Note
            ,GetUtcDate()
            ,GetUtcDate()
        FROM List.[ListItems] li
        INNER JOIN List.[Lists] l on l.Id = li.ParentList_Id
        INNER JOIN List.[NotesHeaders] fh on fh.CustomerNumber = l.CustomerId and fh.BranchId = l.BranchId
        WHERE
            l.Type = 4 and
            li.[Note] is not null and
            li.[Note] != '' and
            NOT EXISTS (
                SELECT
                    'x'
                    FROM [List].[NotesDetails]
                    WHERE [ItemNumber] = li.[ItemNumber]
                        AND [Each] = li.[Each]
                        AND [CatalogId] = li.[CatalogId]
                       AND [HeaderId] = fh.[Id])
        ORDER BY [Id], [ItemNumber]