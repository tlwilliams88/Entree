/****** Object:  Table [ETL].[Staging_ItemData]    Script Date: 10/27/2016 1:05:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [ETL].[Staging_ItemData](
	[BranchId] [char](3) NOT NULL,
	[ItemId] [char](6) NOT NULL,
	[Name] [varchar](30) NULL,
	[Description] [varchar](30) NULL,
	[Brand] [varchar](8) NULL,
	[Pack] [char](4) NULL,
	[Size] [varchar](8) NULL,
	[UPC] [varchar](14) NULL,
	[MfrNumber] [varchar](10) NULL,
	[MfrName] [varchar](30) NULL,
	[Cases] [int] NULL,
	[Package] [int] NULL,
	[PreferredItemCode] [char](1) NULL,
	[CategoryId] [char](6) NULL,
	[ItemType] [char](1) NULL,
	[Status1] [char](1) NULL,
	[Status2] [char](1) NULL,
	[ICSEOnly] [char](1) NULL,
	[SpecialOrderItem] [char](1) NULL,
	[Vendor1] [char](6) NULL,
	[Vendor2] [char](6) NULL,
	[Class] [char](2) NULL,
	[CatMgr] [char](2) NULL,
	[HowPrice] [char](1) NULL,
	[Buyer] [char](2) NULL,
	[Kosher] [char](1) NULL,
	[PVTLbl] [char](1) NULL,
	[MaxSmrt] [char](2) NULL,
	[OrderTiHi] [int] NULL,
	[TiHi] [int] NULL,
	[DateDiscontinued] [int] NULL,
	[Dtelstal] [int] NULL,
	[DTELstPO] [int] NULL,
	[GrossWeight] [int] NULL,
	[NetWeight] [int] NULL,
	[ShelfLife] [int] NULL,
	[DateSensitiveType] [char](1) NULL,
	[Country] [char](10) NULL,
	[Length] [int] NULL,
	[Width] [int] NULL,
	[Height] [int] NULL,
	[Cube] [int] NULL,
	[MinTemp] [int] NULL,
	[MaxTemp] [int] NULL,
	[GDSNSync] [char](1) NULL,
	[GuaranteedDays] [int] NULL,
	[MasterPack] [int] NULL,
	[ReplacementItem] [char](6) NULL,
	[ReplacedItem] [char](6) NULL,
	[TempZone] [char](1) NULL,
	[CNDoc] [char](1) NULL,
	[HACCP] [char](1) NULL,
	[HACCPDoce] [char](5) NULL,
	[FDAProductFlag] [char](1) NULL,
	[FPLength] [int] NULL,
	[FPWidth] [int] NULL,
	[FPHeight] [int] NULL,
	[FPGrossWt] [int] NULL,
	[FPNetWt] [int] NULL,
	[FPCube] [int] NULL,
	[NonStock] [char](1) NULL
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
