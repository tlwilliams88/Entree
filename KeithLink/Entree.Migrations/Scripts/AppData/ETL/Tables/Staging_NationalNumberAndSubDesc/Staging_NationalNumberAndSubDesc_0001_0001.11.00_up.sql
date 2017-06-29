/****** Object:  Table [ETL].[Staging_NationalNumberAndSubDesc]    Script Date: 10/27/2016 1:05:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [ETL].[Staging_NationalNumberAndSubDesc](
	[NationalNumber] [varchar](2) NULL,
	[NationalSub] [varchar](2) NULL,
	[NationalNumberAndSubDesc] [varchar](46) NULL
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
