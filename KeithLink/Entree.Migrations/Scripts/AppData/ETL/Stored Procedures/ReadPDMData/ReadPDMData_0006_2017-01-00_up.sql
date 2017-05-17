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
