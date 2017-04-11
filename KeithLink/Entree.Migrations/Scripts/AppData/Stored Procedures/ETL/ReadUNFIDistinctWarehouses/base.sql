/****** Object:  StoredProcedure [ETL].[ReadUNFIDistinctWarehouses]    Script Date: 10/27/2016 1:05:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE PROCEDURE [ETL].[ReadUNFIDistinctWarehouses]
	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT DISTINCT WarehouseNumber FROM [ETL].Staging_UNFIProducts
END

GO
