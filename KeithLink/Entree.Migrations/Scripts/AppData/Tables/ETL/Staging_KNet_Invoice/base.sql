/****** Object:  Table [ETL].[Staging_KNet_Invoice]    Script Date: 10/27/2016 1:05:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [ETL].[Staging_KNet_Invoice](
	[Action] [varchar](1) NULL,
	[CompanyNumber] [varchar](3) NULL,
	[DivisionNumber] [varchar](3) NULL,
	[DepartmentNumber] [varchar](3) NULL,
	[CustomerNumber] [varchar](10) NULL,
	[OrderNumber] [varchar](9) NULL,
	[LineNumber] [varchar](5) NULL,
	[MemoBillCode] [varchar](3) NULL,
	[CreditOFlag] [varchar](1) NULL,
	[TradeSWFlag] [varchar](1) NULL,
	[ShipDate] [varchar](8) NULL,
	[OrderDate] [varchar](8) NULL,
	[RouteNumber] [varchar](5) NULL,
	[StopNumber] [varchar](3) NULL,
	[WHNumber] [varchar](3) NULL,
	[ItemNumber] [varchar](10) NULL,
	[QuantityOrdered] [varchar](7) NULL,
	[QuantityShipped] [varchar](7) NULL,
	[BrokenCaseCode] [varchar](1) NULL,
	[CatchWeightCode] [varchar](1) NULL,
	[ExtCatchWeight] [varchar](12) NULL,
	[ItemPrice] [varchar](10) NULL,
	[PriceBookNumber] [varchar](5) NULL,
	[ItemPriceSRP] [varchar](12) NULL,
	[OriginalInvoiceNumber] [varchar](20) NULL,
	[InvoiceNumber] [varchar](20) NULL,
	[AC] [varchar](1) NULL,
	[ChangeDate] [varchar](8) NULL,
	[DateOfLastOrder] [varchar](8) NULL,
	[ExtSRPAmount] [varchar](12) NULL,
	[ExtSalesGross] [varchar](16) NULL,
	[ExtSalesNet] [varchar](16) NULL,
	[CustomerGroup] [varchar](10) NULL,
	[SalesRep] [varchar](3) NULL,
	[VendorNumber] [varchar](10) NULL,
	[CustomerPO] [varchar](20) NULL,
	[ChainStoreCode] [varchar](10) NULL,
	[CombStatementCustomer] [varchar](10) NULL,
	[PriceBook] [varchar](7) NULL,
	[ClassCode] [varchar](5) NULL
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
