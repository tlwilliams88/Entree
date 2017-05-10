/****** Object:  Table [ETL].[Staging_FSE_ProductSpec]    Script Date: 10/27/2016 1:05:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING OFF
GO
CREATE TABLE [ETL].[Staging_FSE_ProductSpec](
	[Gtin] [char](14) NULL,
	[PackageGtin] [char](14) NULL,
	[BekItemNumber] [char](6) NULL,
	[Active] [bit] NULL,
	[ProductType] [varchar](20) NULL,
	[ProductShortDesc] [varchar](70) NULL,
	[ProductAdditionalDesc] [varchar](max) NULL,
	[ManufacturerItemNumber] [varchar](30) NULL,
	[UnitsPerCase] [int] NULL,
	[UnitMeasure] [decimal](20, 3) NULL,
	[UnitMeasureUOM] [varchar](3) NULL,
	[GrossWeight] [decimal](20, 3) NULL,
	[NetWeight] [decimal](20, 3) NULL,
	[Length] [decimal](20, 3) NULL,
	[Width] [decimal](20, 3) NULL,
	[Height] [decimal](20, 3) NULL,
	[Volume] [decimal](20, 3) NULL,
	[TiHi] [varchar](250) NULL,
	[Shelf] [int] NULL,
	[StorageTemp] [varchar](35) NULL,
	[ServingsPerPack] [int] NULL,
	[ServingSuggestion] [varchar](max) NULL,
	[MoreInformation] [varchar](35) NULL,
	[MarketingMessage] [varchar](max) NULL,
	[ServingSize] [decimal](20, 3) NULL,
	[ServingSizeUOM] [varchar](3) NULL,
	[Ingredients] [varchar](max) NULL,
	[Brand] [varchar](35) NULL,
	[BrandOwner] [varchar](max) NULL,
	[CountryOfOrigin] [varchar](100) NULL,
	[ContryOfOriginName] [varchar](100) NULL,
	[PreparationInstructions] [varchar](max) NULL,
	[HandlingInstruction] [varchar](max) NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
