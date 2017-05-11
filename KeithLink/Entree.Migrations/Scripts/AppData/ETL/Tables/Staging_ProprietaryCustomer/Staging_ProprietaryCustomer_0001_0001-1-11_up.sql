/****** Object:  Table [ETL].[Staging_ProprietaryCustomer]    Script Date: 10/27/2016 1:05:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [ETL].[Staging_ProprietaryCustomer](
	[Action] [varchar](1) NULL,
	[CompanyNumber] [varchar](3) NULL,
	[DivisionNumber] [varchar](3) NULL,
	[DepartmentNumber] [varchar](3) NULL,
	[ProprietaryNumber] [varchar](10) NULL,
	[CustomerNumber] [varchar](10) NULL
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
