/****** Object:  Table [ETL].[Staging_BidContractDetail]    Script Date: 10/27/2016 1:05:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [ETL].[Staging_BidContractDetail](
	[Action] [varchar](1) NULL,
	[CompanyNumber] [varchar](3) NULL,
	[DivisionNumber] [varchar](3) NULL,
	[DepartmentNumber] [varchar](3) NULL,
	[BidNumber] [varchar](10) NULL,
	[ItemNumber] [varchar](10) NULL,
	[BidLineNumber] [varchar](5) NULL,
	[CategoryNumber] [varchar](5) NULL,
	[CategoryDescription] [varchar](40) NULL,
	[ForceEachOrCaseOnly] [varchar](1) NULL
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Index [ix_INBFCF]    Script Date: 10/27/2016 1:05:25 PM ******/
CREATE NONCLUSTERED INDEX [ix_INBFCF] ON [ETL].[Staging_BidContractDetail]
(
	[DivisionNumber] ASC,
	[BidNumber] ASC
)
INCLUDE ( 	[ItemNumber],
	[BidLineNumber],
	[CategoryDescription],
	[ForceEachOrCaseOnly]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON

GO
