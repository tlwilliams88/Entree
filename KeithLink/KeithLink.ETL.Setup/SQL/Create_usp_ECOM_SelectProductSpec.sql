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
2014-08-01	jmmcmillan	Modified all decimal(20,10) fields to decimal(20,3)
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
	, CONVERT(decimal(20,3), (SELECT Value FROM ItemMeasurement WHERE Gtin = i.Gtin AND MeasurementTypeId = 5 )) 'UnitMeasure' 
	, (SELECT UnitOfMeasure FROM ItemMeasurement WHERE Gtin = i.Gtin AND MeasurementTypeId = 5 ) 'UnitMeasureUOM'
	, CONVERT(decimal(20,3), (SELECT Value FROM ItemMeasurement WHERE Gtin = i.Gtin AND MeasurementTypeId = 0 ))'GrossWeight'
	, CONVERT(decimal(20,3), (SELECT Value FROM ItemMeasurement WHERE Gtin = i.Gtin AND MeasurementTypeId = 1 )) 'NetWeight'	
	, CONVERT(decimal(20,3), (SELECT Value FROM ItemMeasureMent WHERE Gtin = i.Gtin AND MeasureMentTypeId = 2)) 'Length'
	, CONVERT(decimal(20,3), (SELECT Value FROM ItemMeasureMent WHERE Gtin = i.Gtin AND MeasureMentTypeId = 3)) 'Width'
	, CONVERT(decimal(20,3), (SELECT Value FROM ItemMeasureMent WHERE Gtin = i.Gtin AND MeasureMentTypeId = 4)) 'Height'
	, CONVERT(decimal(20,3), (SELECT Value FROM ItemMeasureMent WHERE Gtin = i.Gtin AND MeasureMentTypeId = 7)) 'Volume'
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
	, CONVERT(decimal(20,3), fe.MeasurementValue) 'ServingSize' 
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
