/****** Object:  Table [Orders].[OrderHistoryDetail]    Script Date: 10/27/2016 1:05:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [Orders].[OrderHistoryDetail](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[ItemNumber] [char](6) NULL,
	[LineNumber] [int] NOT NULL,
	[OrderQuantity] [int] NOT NULL,
	[ShippedQuantity] [int] NOT NULL,
	[UnitOfMeasure] [char](1) NULL,
	[CatchWeight] [bit] NOT NULL,
	[ItemDeleted] [bit] NOT NULL,
	[SubbedOriginalItemNumber] [char](6) NULL,
	[ReplacedOriginalItemNumber] [char](6) NULL,
	[ItemStatus] [char](1) NULL,
	[TotalShippedWeight] [decimal](18, 2) NOT NULL,
	[CreatedUtc] [datetime] NOT NULL DEFAULT (getutcdate()),
	[ModifiedUtc] [datetime] NOT NULL DEFAULT (getutcdate()),
	[OrderHistoryHeader_Id] [bigint] NULL,
	[BranchId] [char](3) NULL,
	[InvoiceNumber] [varchar](10) NULL,
	[SellPrice] [decimal](18, 2) NOT NULL DEFAULT ((0)),
	[Source] [char](3) NULL,
	[ManufacturerId] [nvarchar](25) NULL,
	[SpecialOrderHeaderId] [char](7) NULL,
	[SpecialOrderLineNumber] [char](3) NULL,
 CONSTRAINT [PK_Orders.OrderHistoryDetail] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Index [IdxItemUsageGrouping]    Script Date: 10/27/2016 1:05:25 PM ******/
CREATE NONCLUSTERED INDEX [IdxItemUsageGrouping] ON [Orders].[OrderHistoryDetail]
(
	[ItemNumber] ASC,
	[UnitOfMeasure] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 80) ON [PRIMARY]
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [IdxOrderDetail]    Script Date: 10/27/2016 1:05:25 PM ******/
CREATE NONCLUSTERED INDEX [IdxOrderDetail] ON [Orders].[OrderHistoryDetail]
(
	[BranchId] ASC,
	[InvoiceNumber] ASC,
	[LineNumber] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 80) ON [PRIMARY]
GO
/****** Object:  Index [IX_OrderHistoryHeader_Id]    Script Date: 10/27/2016 1:05:25 PM ******/
CREATE NONCLUSTERED INDEX [IX_OrderHistoryHeader_Id] ON [Orders].[OrderHistoryDetail]
(
	[OrderHistoryHeader_Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 80) ON [PRIMARY]
GO
SET ANSI_PADDING ON

GO
