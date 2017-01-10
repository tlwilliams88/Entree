
ALTER PROCEDURE [ETL].[ReadFullItemData]
	@BranchId		varchar(10)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	SELECT 
		BranchId 
		, ItemId 
		, ETL.initcap(Name) as Name
		, ETL.initcap(Description) as Description
		, i.Brand
		, Pack
		, Size
		, UPC
		, MfrNumber
		, MfrName
		, (SELECT b.ExtendedDescription FROM ETL.Staging_Brands b WHERE b.Brand = i.Brand) As BrandDescription
		, MaxSmrt
		, Cases
		, Package
		, PreferredItemCode
		, ItemType
		, Status1
		, Status2
		, ICSEOnly
		, SpecialOrderItem
		, Vendor1
		, Vendor2
		, Class
		, CatMgr
		, HowPrice
		, Buyer
		, Kosher
		, c.CategoryId
		, ReplacementItem
		, ReplacedItem
		, CNDoc
		, [Cube]
		, ETL.initcap(c.CategoryName) as CategoryName
		, (SELECT CategoryId from ETL.Staging_Category WHERE CategoryId = SUBSTRING(c.CategoryId, 1, 2) + '000') as ParentCategoryId
		, (SELECT ETL.initcap(CategoryName) from ETL.Staging_Category WHERE CategoryId = SUBSTRING(c.CategoryId, 1, 2) + '000') as ParentCategoryName
		, ps.BrandOwner
		, ps.CountryOfOrigin
		, ps.GrossWeight
		, ps.HandlingInstruction
		, ps.Height
		, ps.Ingredients
		, ps.Length
		, ps.MarketingMessage
		, ps.MoreInformation
		, ps.ServingSize
		, ps.ServingSizeUOM
		, ps.ServingsPerPack
		, ps.ServingSuggestion
		, ps.Shelf
		, ps.StorageTemp
		, ps.UnitMeasure
		, ps.UnitsPerCase
		, ps.Volume
		, ps.Width
		, CASE i.NonStock
			WHEN 'C' THEN 'Y'
			WHEN 'N' THEN 'Y'
			WHEN 'Y' THEN 'N'
			WHEN 'S' THEN 'N'
			WHEN ' ' THEN 'N'
		ELSE 'Y' END AS 'NonStock'
		, i.FDAProductFlag
		, i.TempZone
		, i.FPNetWt
		, i.GrossWeight 'GrossWt'	 
	FROM  
		ETL.Staging_ItemData i inner join 
		ETL.Staging_Category c on i.CategoryId = c.CategoryId left outer join
		ETL.Staging_FSE_ProductSpec ps on i.UPC = ps.Gtin
	WHERE 
		  i.ItemId NOT LIKE '999%'  AND SpecialOrderItem <>'Y' and BranchId = @BranchId
END