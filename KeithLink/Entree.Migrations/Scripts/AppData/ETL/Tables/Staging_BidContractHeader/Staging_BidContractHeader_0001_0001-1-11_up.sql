/****** Object:  Table [ETL].[Staging_BidContractHeader]    Script Date: 10/27/2016 1:05:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [ETL].[Staging_BidContractHeader](
	[Action] [varchar](1) NULL,
	[CompanyNumber] [varchar](3) NULL,
	[DivisionNumber] [varchar](3) NULL,
	[DepartmentNumber] [varchar](3) NULL,
	[BidNumber] [varchar](10) NULL,
	[BidDescription] [varchar](40) NULL
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
