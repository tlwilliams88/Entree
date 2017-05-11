/****** Object:  StoredProcedure [ETL].[ReadUNFICategories]    Script Date: 10/27/2016 1:05:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE PROCEDURE [ETL].[ReadUNFICategories]
	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT DISTINCT SUBSTRING(TCSCode, 1, 3) + '00' AS CategoryId,[ETL].initcap(Category) as CategoryName, [Type] AS Department FROM [ETL].Staging_UNFIProducts
END

GO
