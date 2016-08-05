﻿/*
* Retieve BEK ItemNumber by VendorId using External Catalogs for branch
* 
* Author: mdjoiner
* Changed: 2016-05-13
*/
CREATE PROCEDURE [ETL].[ProcessBEKItemNumbersForUNFIByVendorId]
    @vendorId varchar(10)
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE
        unfi
    SET
        unfi.ProductNumber = item.ItemId,
        unfi.StockedInBranches = ETL.ConcatBranchesForItem(@vendorId, item.ItemId)
    FROM
        ETL.Staging_UNFIProducts unfi
        JOIN ETL.Staging_ItemData item ON item.MfrNumber = unfi.ProductNumber
    WHERE
        item.Vendor1 = @vendorId
    AND
        item.SpecialOrderItem = 'N'
END