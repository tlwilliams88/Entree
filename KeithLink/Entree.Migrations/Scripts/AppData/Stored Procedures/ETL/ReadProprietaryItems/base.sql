/****** Object:  StoredProcedure [ETL].[ReadProprietaryItems]    Script Date: 10/27/2016 1:05:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [ETL].[ReadProprietaryItems]
	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	SELECT
		DISTINCT
		c.CustomerNumber,
		i.ItemNumber
	FROM
		ETL.Staging_ProprietaryItem i INNER JOIN
		ETL.Staging_ProprietaryCustomer c on i.ProprietaryNumber = c.ProprietaryNumber
	Order By
		i.ItemNumber
END


GO
