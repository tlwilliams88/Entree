/****** Object:  StoredProcedure [ETL].[ReadItemGS1Data]    Script Date: 10/27/2016 1:05:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [ETL].[ReadItemGS1Data]
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT
		n.DailyValue,
		n.Gtin,
		n.MeasurementValue,
		n.MeasurmentTypeId,
		n.NutrientTypeCode,
		n.NutrientTypeDesc
	FROM
		ETL.Staging_FSE_ProductNutrition n
	WHERE
		n.DailyValue IS NOT NULL

	SELECT
		d.Gtin,
		d.DietType,
		d.Value
	FROM
		ETL.Staging_FSE_ProductDiet d
	WHERE
		d.Value IS NOT NULL

	SELECT
		a.Gtin,
		a.AllergenTypeCode,
		a.AllergenTypeDesc,
		a.LevelOfContainment		
	FROM
		ETL.Staging_FSE_ProductAllergens a
	WHERE
		a.AllergenTypeDesc IS NOT NULL
END



GO
