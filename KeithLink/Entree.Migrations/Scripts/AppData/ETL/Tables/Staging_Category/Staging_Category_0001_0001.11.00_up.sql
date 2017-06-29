/****** Object:  Table [ETL].[Staging_Category]    Script Date: 10/27/2016 1:05:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [ETL].[Staging_Category](
	[CategoryId] [char](6) NULL,
	[CategoryName] [varchar](50) NULL,
	[PPICode] [char](8) NULL
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
