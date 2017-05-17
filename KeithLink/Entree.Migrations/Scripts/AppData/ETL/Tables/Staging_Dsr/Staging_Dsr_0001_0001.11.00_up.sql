/****** Object:  Table [ETL].[Staging_Dsr]    Script Date: 10/27/2016 1:05:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [ETL].[Staging_Dsr](
	[DsrNumber] [char](8) NOT NULL,
	[Name] [varchar](50) NOT NULL,
	[EmailAddress] [varchar](50) NOT NULL
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
