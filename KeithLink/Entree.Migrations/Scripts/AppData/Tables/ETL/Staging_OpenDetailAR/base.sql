/****** Object:  Table [ETL].[Staging_OpenDetailAR]    Script Date: 10/27/2016 1:05:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [ETL].[Staging_OpenDetailAR](
	[Action] [varchar](50) NULL,
	[Company] [varchar](50) NULL,
	[Division] [varchar](50) NULL,
	[Department] [varchar](3) NULL,
	[Customer] [varchar](10) NULL,
	[OriginalInvoiceNumber] [varchar](20) NULL,
	[InvoiceNumber] [varchar](20) NULL,
	[AC] [varchar](1) NULL,
	[ChangeDate] [varchar](8) NULL,
	[ReclineNumber] [varchar](9) NULL,
	[InvoiceType] [varchar](1) NULL,
	[DateOfLastOrder] [varchar](8) NULL,
	[InvoiceAmount] [varchar](16) NULL,
	[CheckNumber] [varchar](9) NULL,
	[AdjustCode] [varchar](3) NULL,
	[InvoiceReference] [varchar](20) NULL,
	[InvoiceDue] [varchar](8) NULL,
	[CombinedStmt] [varchar](10) NULL
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
