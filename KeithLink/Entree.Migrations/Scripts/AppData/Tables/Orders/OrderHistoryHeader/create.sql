/****** Object:  Table [Orders].[OrderHistoryHeader]    Script Date: 10/27/2016 1:05:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [Orders].[OrderHistoryHeader](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[OrderSystem] [char](1) NULL,
	[BranchId] [char](3) NULL,
	[CustomerNumber] [char](6) NULL,
	[InvoiceNumber] [varchar](10) NULL,
	[PONumber] [nvarchar](20) NULL,
	[ControlNumber] [char](7) NULL,
	[OrderStatus] [char](1) NULL,
	[FutureItems] [bit] NOT NULL,
	[ErrorStatus] [bit] NOT NULL,
	[RouteNumber] [char](4) NULL,
	[CreatedUtc] [datetime] NOT NULL DEFAULT (getutcdate()),
	[ModifiedUtc] [datetime] NOT NULL DEFAULT (getutcdate()),
	[StopNumber] [char](3) NULL,
	[DeliveryOutOfSequence] [bit] NULL,
	[OriginalControlNumber] [char](7) NULL,
	[IsSpecialOrder] [bit] NOT NULL DEFAULT ((0)),
	[RelatedControlNumber] [char](7) NULL,
	[DeliveryDate] [char](10) NULL,
	[ScheduledDeliveryTime] [char](19) NULL,
	[EstimatedDeliveryTime] [char](19) NULL,
	[ActualDeliveryTime] [char](19) NULL,
	[OrderSubtotal] [decimal](18, 2) NOT NULL DEFAULT ((0)),
 CONSTRAINT [PK_Orders.OrderHistoryHeader] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 80) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Index [IdxCustomerNumberByDate]    Script Date: 10/27/2016 1:05:25 PM ******/
CREATE NONCLUSTERED INDEX [IdxCustomerNumberByDate] ON [Orders].[OrderHistoryHeader]
(
	[CustomerNumber] ASC,
	[DeliveryDate] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 80) ON [PRIMARY]
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [IdxOrderHeader]    Script Date: 10/27/2016 1:05:25 PM ******/
CREATE NONCLUSTERED INDEX [IdxOrderHeader] ON [Orders].[OrderHistoryHeader]
(
	[BranchId] ASC,
	[InvoiceNumber] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 80) ON [PRIMARY]
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [ix_OrderHistoryheader_OrderSystem_includes]    Script Date: 10/27/2016 1:05:25 PM ******/
CREATE NONCLUSTERED INDEX [ix_OrderHistoryheader_OrderSystem_includes] ON [Orders].[OrderHistoryHeader]
(
	[OrderSystem] ASC
)
INCLUDE ( 	[Id],
	[BranchId],
	[CustomerNumber],
	[InvoiceNumber],
	[PONumber],
	[ControlNumber],
	[OrderStatus],
	[FutureItems],
	[ErrorStatus],
	[RouteNumber],
	[CreatedUtc],
	[ModifiedUtc],
	[StopNumber],
	[DeliveryOutOfSequence],
	[OriginalControlNumber],
	[IsSpecialOrder],
	[RelatedControlNumber],
	[DeliveryDate],
	[ScheduledDeliveryTime],
	[EstimatedDeliveryTime],
	[ActualDeliveryTime],
	[OrderSubtotal]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 100) ON [PRIMARY]
GO
