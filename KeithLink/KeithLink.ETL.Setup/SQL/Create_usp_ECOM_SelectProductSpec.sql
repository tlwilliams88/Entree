USE FSE;
GO

CREATE PROCEDURE dbo.usp_ECOM_SelectProductSpec AS

/*******************************************************************
* PROCEDURE: usp_ECOM_SelectProductSpec
* PURPOSE: Select fields for ecom stage Stagin_FSE_ProductSpec table
* NOTES: {special set up or requirements, etc.}
* CREATED:	Jason McMillan 7-25-14
* MODIFIED 
* DATE		AUTHOR			DESCRIPTION
*-------------------------------------------------------------------
* {date}	{developer}	{brief modification description}
*******************************************************************/

SET NOCOUNT ON;

SELECT
	DISTINCT
	i.Gtin 'Gtin'
	, NULL 'PackageGtin'
	, i.ItemNumber 'BekItemNumber'
	, i.Active
	, dbo.GetItemType(i.Gtin) 'ProductType'
	, id.DescriptionShort 'ProductShortDesc'
	, id.AdditionalDescription 'ProductAdditionalDesc'
	, ii.Code 'ManufacturerItemNumber' 
	, ih.TotalLowerLevelItems 'UnitsPerCase'
	, (SELECT Value FROM ItemMeasurement WHERE Gtin = i.Gtin AND MeasurementTypeId = 5 ) 'UnitMeasure' 
	, (SELECT UnitOfMeasure FROM ItemMeasurement WHERE Gtin = i.Gtin AND MeasurementTypeId = 5 ) 'UnitMeasureUOM'
	, (SELECT Value FROM ItemMeasurement WHERE Gtin = i.Gtin AND MeasurementTypeId = 0 )'GrossWeight'
	, (SELECT Value FROM ItemMeasurement WHERE Gtin = i.Gtin AND MeasurementTypeId = 1 ) 'NetWeight'	
	, (SELECT Value FROM ItemMeasureMent WHERE Gtin = i.Gtin AND MeasureMentTypeId = 2) 'Length'
	, (SELECT Value FROM ItemMeasureMent WHERE Gtin = i.Gtin AND MeasureMentTypeId = 3) 'Width'
	, (SELECT Value FROM ItemMeasureMent WHERE Gtin = i.Gtin AND MeasureMentTypeId = 4) 'Height'
	, (SELECT Value FROM ItemMeasureMent WHERE Gtin = i.Gtin AND MeasureMentTypeId = 7) 'Volume'
	, CONCAT(ih.ItemsPerPalletlayer, 'x', ih.LayersPerPallet) 'TiHi'
	, ist.MinimumLifespanFromProduction 'Shelf'
	, CONCAT( 
				(SELECT CONCAT(CAST(MinTemperature AS int), ' ',  MinTemperatureUOM) FROM ItemStorage WHERE Gtin = i.Gtin)
				, ' / '
				, (SELECT CONCAT(CAST(MaxTemperature AS int), ' ', MaxTemperatureUOM) FROM ItemStorage WHERE Gtin = i.Gtin) 
	) 'StorageTemp'
	, im.ServingsPerPack
	, im.ServingSuggestion
	, im.MoreInformation
	, im.MarketingMessage
	, fe.MeasurementValue 'ServingSize' 
	, fe.MeasurementTypeId 'ServingSizeUOM'
	, fe.Ingredients 'Ingredients'
	, id.BrandName 'Brand'
	, pty.Name 'BrandOwner' 
	, id.CountryOfOrigin 'Country of Origin'
	, co.Name 'CountryOfOriginName'
	, prp.Description 'PreparationInstructions'
	, si.HandlingInstructions

FROM
	Item i
	LEFT OUTER JOIN FoodExtension fe ON fe.Gtin = i.Gtin
	LEFT OUTER JOIN ItemDescription id ON id.Gtin = i.Gtin
	LEFT OUTER JOIN ItemDiet idt ON idt.Gtin = i.Gtin
	LEFT OUTER JOIN ItemHierarchy ih ON ih.Gtin = i.Gtin
	LEFT OUTER JOIN ItemIdentification ii ON ii.Gtin = i.Gtin
	LEFT OUTER JOIN ItemStorage ist ON ist.Gtin = i.Gtin
	LEFT OUTER JOIN PartyIdentification pty ON i.BrandOwnerGLN = pty.GLN
	LEFT OUTER JOIN ItemMarketing im ON im.Gtin = i.Gtin
	LEFT OUTER JOIN PreparationInstructions prp ON prp.Gtin = i.Gtin
	LEFT OUTER JOIN StorageInstructions si ON si.Gtin = i.Gtin
	LEFT OUTER JOIN CountryOfOrigin co ON id.CountryOfOrigin = co.CountryCode

WHERE	
	ii.AlternateIdentificationTypeId = 0
	AND i.Active = 1
