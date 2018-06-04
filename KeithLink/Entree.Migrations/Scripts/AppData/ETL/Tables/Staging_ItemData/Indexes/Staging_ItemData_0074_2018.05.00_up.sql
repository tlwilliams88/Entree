CREATE CLUSTERED INDEX IX_StagingItemData_ItemIdBranchId
    ON ETL.Staging_ItemData (ItemId, BranchId)