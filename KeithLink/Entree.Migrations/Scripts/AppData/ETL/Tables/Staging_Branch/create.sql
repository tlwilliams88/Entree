/****** Object:  Table [ETL].[Staging_Branch]    Script Date: 10/27/2016 1:05:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [ETL].[Staging_Branch](
	[BranchId] [char](10) NULL,
	[LocationTypeId] [bigint] NULL,
	[Description] [varchar](50) NULL
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
