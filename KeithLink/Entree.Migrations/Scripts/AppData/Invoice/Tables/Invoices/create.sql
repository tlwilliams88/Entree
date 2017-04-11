/****** Object:  Table [Invoice].[Invoices]    Script Date: 10/27/2016 1:05:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [Invoice].[Invoices](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[InvoiceNumber] [nvarchar](max) NULL,
	[OrderDate] [datetime] NULL,
	[CustomerNumber] [nvarchar](max) NULL,
	[CreatedUtc] [datetime] NOT NULL,
	[ModifiedUtc] [datetime] NOT NULL,
	[BranchId] [nvarchar](3) NULL,
	[InvoiceDate] [datetime] NULL,
	[Type] [int] NOT NULL,
	[Amount] [decimal](18, 2) NOT NULL,
	[Status] [int] NOT NULL,
	[DueDate] [datetime] NULL,
 CONSTRAINT [PK_Invoice.Invoices] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
ALTER TABLE [Invoice].[Invoices] ADD  DEFAULT (getutcdate()) FOR [CreatedUtc]
GO
ALTER TABLE [Invoice].[Invoices] ADD  DEFAULT (getutcdate()) FOR [ModifiedUtc]
GO
ALTER TABLE [Invoice].[Invoices] ADD  DEFAULT ((0)) FOR [Type]
GO
ALTER TABLE [Invoice].[Invoices] ADD  DEFAULT ((0)) FOR [Amount]
GO
ALTER TABLE [Invoice].[Invoices] ADD  DEFAULT ((0)) FOR [Status]
GO
