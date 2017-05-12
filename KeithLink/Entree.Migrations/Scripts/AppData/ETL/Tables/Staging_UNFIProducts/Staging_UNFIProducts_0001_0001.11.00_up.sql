/****** Object:  Table [ETL].[Staging_UNFIProducts]    Script Date: 10/27/2016 1:05:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [ETL].[Staging_UNFIProducts](
	[WarehouseNumber] [int] NULL,
	[Description] [varchar](150) NULL,
	[Brand] [varchar](150) NULL,
	[CLength] [decimal](18, 0) NULL,
	[CWidth] [decimal](9, 3) NULL,
	[CHeight] [decimal](9, 3) NULL,
	[TempControl] [varchar](50) NULL,
	[UnitOfSale] [varchar](50) NULL,
	[CatalogDept] [varchar](50) NULL,
	[ShipMinExpire] [varchar](50) NULL,
	[ProductNumber] [varchar](10) NULL,
	[MinOrder] [int] NULL,
	[VendorCasesPerTier] [int] NULL,
	[VendorTiersPerPallet] [int] NULL,
	[VendorCasesPerPallet] [int] NULL,
	[CaseQuantity] [int] NULL,
	[PutUp] [varchar](50) NULL,
	[ContSize] [decimal](9, 3) NULL,
	[ContUnit] [varchar](50) NULL,
	[TCSCode] [varchar](50) NULL,
	[RetailUPC] [varchar](50) NULL,
	[CaseUPC] [varchar](50) NULL,
	[Weight] [decimal](9, 3) NULL,
	[PLength] [decimal](9, 3) NULL,
	[PHeight] [decimal](9, 3) NULL,
	[PWidth] [decimal](9, 3) NULL,
	[Status] [varchar](50) NULL,
	[Type] [varchar](50) NULL,
	[Category] [varchar](50) NULL,
	[Subgroup] [varchar](50) NULL,
	[EachPrice] [decimal](9, 2) NULL,
	[CasePrice] [decimal](9, 2) NULL,
	[Flag1] [varchar](50) NULL,
	[Flag2] [varchar](50) NULL,
	[Flag3] [varchar](50) NULL,
	[Flag4] [varchar](50) NULL,
	[OnHandQty] [int] NULL,
	[Vendor] [varchar](150) NULL,
	[StockedInBranches] [varchar](50) NULL
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
