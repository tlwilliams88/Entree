/**
*
* -------------------------------------------------------
* Read the item data for the elastic search load. 
* This proc is used by the internalsvc load process.
* -------------------------------------------------------
*   Changed  | Migration | By       
* -------------------------------------------------------
* 2017-03-03 | 13        | mdjoiner
*
**/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROCEDURE [ETL].[ReadFullItemData]
	@BranchId		varchar(10)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	SELECT 
		BranchId 
		,ItemId 
		,ETL.initcap(Name) as Name
		,ETL.initcap(Description) as Description
		,i.Brand
		,Pack
		,Size
		,UPC
		,MfrNumber
		,MfrName
		,(SELECT b.ExtendedDescription FROM ETL.Staging_Brands b WHERE b.Brand = i.Brand) As BrandDescription
		,MaxSmrt
		,Cases
		,Package
		,PreferredItemCode
		,ItemType
		,Status1
		,Status2
		,ICSEOnly
		,SpecialOrderItem
		,Vendor1
		,Vendor2
		,Class
		,CatMgr
		,HowPrice
		,Buyer
		,Kosher
		,i.CategoryId
		,ReplacementItem
		,ReplacedItem
		,CNDoc
		,[Cube]
		,ETL.initcap(c.DepartmentName) as CategoryName
		,c.ParentDepartment as ParentCategoryId
		,(SELECT ETL.initcap(DepartmentName) from ETL.Staging_Departments WHERE DepartmentId = c.ParentDepartment) as ParentCategoryName
		,ps.BrandOwner
		,ps.CountryOfOrigin
		,ps.GrossWeight
		,ps.HandlingInstruction
		,ps.Height
		,ps.Ingredients
		,ps.Length
		,ps.MarketingMessage
		,ps.MoreInformation
		,ps.ServingSize
		,ps.ServingSizeUOM
		,ps.ServingsPerPack
		,ps.ServingSuggestion
		,ps.Shelf
		,ps.StorageTemp
		,ps.UnitMeasure
		,ps.UnitsPerCase
		,ps.Volume
		,ps.Width
		,CASE i.NonStock
			WHEN 'C' THEN 'Y'
			WHEN 'N' THEN 'Y'
			WHEN 'Y' THEN 'N'
			WHEN 'S' THEN 'N'
			WHEN ' ' THEN 'N'
		ELSE 'Y' END AS 'NonStock'
		,i.FDAProductFlag
		,i.TempZone
		,i.FPNetWt
		,i.GrossWeight 'GrossWt'
		,b.ControlLabel AS [BrandControlLabel]
        ,i.PriceListCode
	FROM  
		ETL.Staging_ItemData i 
		inner join ETL.Staging_Departments c on i.CategoryId = c.DepartmentId
		left outer join	ETL.Staging_FSE_ProductSpec ps on i.UPC = ps.Gtin
		left outer join ETL.Staging_Brands b on b.Brand = i.Brand
	WHERE 
		  i.ItemId NOT LIKE '999%'  AND SpecialOrderItem <>'Y' and BranchId = @BranchId
END