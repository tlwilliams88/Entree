/****** Object:  Table [Customers].[ItemHistory]    Script Date: 10/27/2016 1:05:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [Customers].[ItemHistory](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[BranchId] [char](3) NULL,
	[CustomerNumber] [char](6) NULL,
	[ItemNumber] [char](6) NULL,
	[CreatedUtc] [datetime] NOT NULL DEFAULT (getutcdate()),
	[ModifiedUtc] [datetime] NOT NULL DEFAULT (getutcdate()),
	[UnitOfMeasure] [char](1) NULL,
	[AverageUse] [int] NOT NULL DEFAULT ((0)),
 CONSTRAINT [PK_Customers.ItemHistory] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Index [IdxItemHistory]    Script Date: 10/27/2016 1:05:25 PM ******/
CREATE NONCLUSTERED INDEX [IdxItemHistory] ON [Customers].[ItemHistory]
(
	[BranchId] ASC,
	[CustomerNumber] ASC,
	[ItemNumber] ASC,
	[UnitOfMeasure] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
