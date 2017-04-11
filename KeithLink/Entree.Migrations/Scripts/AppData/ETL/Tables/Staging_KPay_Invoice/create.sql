/****** Object:  Table [ETL].[Staging_KPay_Invoice]    Script Date: 10/27/2016 1:05:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [ETL].[Staging_KPay_Invoice](
	[InvoiceNumber] [varchar](30) NOT NULL,
	[Division] [char](5) NOT NULL,
	[CustomerNumber] [char](6) NOT NULL,
	[ItemSequence] [smallint] NOT NULL,
	[InvoiceType] [char](3) NOT NULL,
	[InvoiceDate] [datetime] NOT NULL,
	[DueDate] [datetime] NOT NULL,
	[AmountDue] [decimal](9, 2) NOT NULL,
	[DeleteFlag] [bit] NOT NULL
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
