USE FSE;
GO

CREATE PROCEDURE dbo.usp_ECOM_SelectProductDiet AS

/*******************************************************************
* PROCEDURE: usp_ECOM_SelectProductDiet
* PURPOSE: Select fields for ecom stage Stagin_FSE_ProductDiet table
* NOTES: {special set up or requirements, etc.}
* CREATED:	Jason McMillan 7-25-14
* MODIFIED 
* DATE		AUTHOR			DESCRIPTION
*-------------------------------------------------------------------
* {date}	{developer}	{brief modification description}
*******************************************************************/

SET NOCOUNT ON;

DECLARE @DietTypeValues TABLE(
	Gtin char(14)
	, DietType varchar(25)
	, Value varchar(1)	
);

DECLARE c_Gtin CURSOR FOR 
	SELECT 
		DISTINCT
		Gtin
	FROM
		Item

DECLARE @Gtin char(14);

OPEN c_Gtin;

FETCH NEXT FROM c_Gtin INTO @Gtin;

WHILE @@FETCH_STATUS = 0
BEGIN
	INSERT INTO @DietTypeValues
	SELECT
		@Gtin
		, DietType
		, CASE
			WHEN Value = 0 THEN 'n'
			WHEN Value = 1 THEN 'y'
			ELSE NULL
		END
	FROM
		ItemDiet
	WHERE
		Gtin = @Gtin
		AND Value IS NOT NULL
	
	FETCH NEXT FROM c_Gtin INTO @Gtin;

END

CLOSE c_Gtin;
DEALLOCATE c_Gtin;

SELECT 
	Gtin
	, DietType
	, Value 
FROM 
	@DietTypeValues
