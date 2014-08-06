USE FSE;

DECLARE @uspCountProductSpec int;
SET @uspCountProductSpec = (
	SELECT 
		COUNT(*) 
	FROM 
		dbo.sysobjects so
	WHERE
		so.name = 'usp_ECOM_SelectProductSpec'
);

DECLARE @uspCountProductNutrition int;
SET @uspCountProductNutrition = (
	SELECT 
		COUNT(*) 
	FROM 
		dbo.sysobjects so
	WHERE
		so.name = 'usp_ECOM_SelectProductNutrition'
);


DECLARE @uspCountProductDiet int;
SET @uspCountProductDiet = (
	SELECT 
		COUNT(*) 
	FROM 
		dbo.sysobjects so
	WHERE
		so.name = 'usp_ECOM_SelectProductDiet'
);

DECLARE @uspCountProductAllergens int;
SET @uspCountProductAllergens = (
	SELECT 
		COUNT(*) 
	FROM 
		dbo.sysobjects so
	WHERE
		so.name = 'usp_ECOM_SelectProductNutrition'
);

DECLARE @uspCountGetItemType int;
SET @uspCountGetItemType = (
	SELECT 
		COUNT(*) 
	FROM 
		dbo.sysobjects so
	WHERE
		so.name = 'GetItemType'
);


SELECT
	@uspCountProductSpec 'ProductSpec'
	, @uspCountProductNutrition 'ProductNutrition'
	, @uspCountProductDiet 'ProductDiet'
	, @uspCountProductAllergens 'ProductAllergens'
	, @uspCountGetItemType 'GetItemType';


GO