/****** Object:  StoredProcedure [ETL].[ReadSubCategories]    Script Date: 10/27/2016 1:05:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [ETL].[ReadSubCategories]
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	SELECT 
		SUBSTRING(CategoryId, 1, 2) + '000' AS ParentCategoryId, 
		CategoryId, 
		[ETL].initcap(CategoryName) as CategoryName, 
		PPICode 
	FROM 
		[ETL].Staging_Category 
	WHERE 
		CategoryId not like '%000'
END





GO
