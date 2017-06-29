/****** Object:  Table [Invoice].[InvoiceItems]    Script Date: 10/27/2016 1:05:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [Invoice].[InvoiceItems](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[QuantityOrdered] [int] NULL,
	[QuantityShipped] [int] NULL,
	[CatchWeightCode] [bit] NOT NULL,
	[ExtCatchWeight] [decimal](18, 2) NULL,
	[ItemPrice] [decimal](18, 2) NULL,
	[ExtSalesNet] [decimal](18, 2) NULL,
	[CreatedUtc] [datetime] NOT NULL,
	[ModifiedUtc] [datetime] NOT NULL,
	[InvoiceId] [bigint] NOT NULL,
	[ItemNumber] [nvarchar](10) NULL,
	[ClassCode] [char](2) NULL,
	[LineNumber] [nvarchar](6) NULL,
 CONSTRAINT [PK_Invoice.InvoiceItems] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

ALTER TABLE [Invoice].[InvoiceItems] ADD  DEFAULT (getutcdate()) FOR [CreatedUtc]
GO
ALTER TABLE [Invoice].[InvoiceItems] ADD  DEFAULT (getutcdate()) FOR [ModifiedUtc]
