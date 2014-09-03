USE [FSE];
GO

CREATE PROCEDURE [dbo].[usp_ECOM_SelectProductNutrition] AS

/*******************************************************************
* PROCEDURE: usp_ECOM_SelectProductNutrition
* PURPOSE: Select fields for ecom stage Stagin_FSE_ProductNutrition table
* NOTES: {special set up or requirements, etc.}
* CREATED:	Jason McMillan 7-25-14
* MODIFIED 
* DATE		AUTHOR			DESCRIPTION
*-------------------------------------------------------------------
* {date}	{developer}	{brief modification description}
2014-08-21	jmmcmillan	Changed MeasurementValue to decimal(20,3) per Phil B. and Mark P.
*******************************************************************/

SELECT
	DISTINCT
	i.Gtin
	, ni.NutrientTypeCode
	, ntc.Description 'NutrientTypeDesc'
	, ni.MeasurementTypeId
	, CONVERT(decimal(20,3), ni.MeasurementValue) 'MeasurementValue'
	, ni.DailyValue
FROM
	Item i
	LEFT OUTER JOIN NutrientInformation ni ON ni.Gtin = i.Gtin
		LEFT OUTER JOIN NutrientType ntc ON ni.NutrientTypeCode = ntc.NutrientCode

