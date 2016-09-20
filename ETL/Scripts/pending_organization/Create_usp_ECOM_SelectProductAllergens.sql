USE FSE;
GO

CREATE PROCEDURE dbo.usp_ECOM_SelectProductAllergens AS

/*******************************************************************
* PROCEDURE: usp_ECOM_SelectProductAllergens
* PURPOSE: Select fields for ecom stage Stagin_FSE_ProductSpec table
* NOTES: {special set up or requirements, etc.}
* CREATED:	Jason McMillan 7-25-14
* MODIFIED 
* DATE		AUTHOR			DESCRIPTION
*-------------------------------------------------------------------
* {date}	{developer}	{brief modification description}
*******************************************************************/

SELECT
	DISTINCT
	i.Gtin
	, fa.AllergenTypeCode
	, a.Description 'Allergen.Description'
	, CASE
		WHEN fa.LevelOfContainment = '' OR fa.LevelOfContainment IS NULL THEN NULL
		ELSE fa.LevelOfContainment
	END AS 'LevelOfContainment'
FROM
	Item i
	LEFT OUTER JOIN FoodAllergen fa ON i.Gtin = fa.Gtin
		LEFT OUTER JOIN Allergen a ON fa.AllergenTypeCode = a.AllergenTypeCode

