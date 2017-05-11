/****** Object:  Table [ETL].[Staging_CustomerBid]    Script Date: 10/27/2016 1:05:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [ETL].[Staging_CustomerBid](
	[Action] [varchar](1) NULL,
	[CompanyNumber] [varchar](3) NULL,
	[DivisionNumber] [varchar](3) NULL,
	[DepartmentNumber] [varchar](3) NULL,
	[CustomerNumber] [varchar](10) NULL,
	[PriorityNumber] [varchar](3) NULL,
	[BidNumber] [varchar](10) NULL
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Index [IX_Stg_CustBid_BidNumver_DivisionNumber]    Script Date: 10/27/2016 1:05:25 PM ******/
CREATE NONCLUSTERED INDEX [IX_Stg_CustBid_BidNumver_DivisionNumber] ON [ETL].[Staging_CustomerBid]
(
	[BidNumber] ASC,
	[DivisionNumber] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 80) ON [PRIMARY]
GO
SET ANSI_PADDING ON

GO
