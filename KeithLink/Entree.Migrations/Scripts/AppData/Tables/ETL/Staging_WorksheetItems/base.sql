/****** Object:  Table [ETL].[Staging_WorksheetItems]    Script Date: 10/27/2016 1:05:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [ETL].[Staging_WorksheetItems](
	[Action] [varchar](1) NULL,
	[CompanyNumber] [varchar](3) NULL,
	[DivisionNumber] [varchar](3) NULL,
	[DepartmentNumber] [varchar](3) NULL,
	[CustomerNumber] [varchar](10) NULL,
	[ItemNumber] [varchar](10) NULL,
	[BrokenCaseCode] [varchar](1) NULL,
	[ItemPrice] [varchar](10) NULL,
	[QtyOrdered] [varchar](7) NULL,
	[DateOfLastOrder] [varchar](8) NULL
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Index [IX_CustDiv]    Script Date: 10/27/2016 1:05:25 PM ******/
CREATE NONCLUSTERED INDEX [IX_CustDiv] ON [ETL].[Staging_WorksheetItems]
(
	[DivisionNumber] ASC,
	[CustomerNumber] ASC
)
INCLUDE ( 	[ItemNumber],
	[BrokenCaseCode]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
