/****** Object:  StoredProcedure [ETL].[ReadParentCategories]    Script Date: 10/27/2016 1:05:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE PROC [ETL].[ReadPDMData]
AS
	SET NOCOUNT ON;

	SELECT
		p.[ItemNumber],
		p.[Status],
		p.[Name],
		p.[Brand],
		p.[Manufacturer],
		p.[Description],
		p.[CreatedBy],
		p.[CreatedUTC],
		p.[UpdatedBy],
		p.[ModifiedUTC]
	FROM
		[ETL].[Staging_PDM_EnrichedProducts] p
GO
