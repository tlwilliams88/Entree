/****** Object:  StoredProcedure [ETL].[ReadUNFISubCategories]    Script Date: 10/27/2016 1:05:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE PROCEDURE [ETL].[ReadUNFISubCategories]
	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT DISTINCT SUBSTRING(TCSCode, 1, 3) + '00' AS ParentCategoryId,TCSCode as CategoryId,[ETL].initcap(SubGroup) as CategoryName, [Type] as Department FROM [ETL].Staging_UNFIProducts
END

GO
