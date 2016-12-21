CREATE TABLE [ETL].[Staging_PDM_EnrichedProducts] (
	[CreatedUTC]	DATETIME		NOT NULL,
	[ModifiedUTC]	DATETIME		NOT NULL,
	[ItemNumber]	VARCHAR(6)		NOT NULL,
	[Status]		VARCHAR(20),
	[Brand]			NVARCHAR(40),
	[Manufacturer]	NVARCHAR(40),
	[Name]			NVARCHAR(50),
	[CreatedBy]		VARCHAR(256)	NOT NULL,
	[UpdatedBy]		VARCHAR(256)	NOT NULL,
	[Description]	NVARCHAR(1000)
)