/****** Object:  Table [ETL].[Staging_Terms]    Script Date: 10/27/2016 1:05:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [ETL].[Staging_Terms](
	[Action] [varchar](1) NULL,
	[Company] [varchar](3) NULL,
	[Code] [varchar](3) NULL,
	[Description] [varchar](25) NULL,
	[Age1] [varchar](3) NULL,
	[Age2] [varchar](3) NULL,
	[Age3] [varchar](3) NULL,
	[Age4] [varchar](3) NULL,
	[Prox] [varchar](1) NULL
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
