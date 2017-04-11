/****** Object:  Table [ETL].[Staging_FSE_ProductDiet]    Script Date: 10/27/2016 1:05:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING OFF
GO
CREATE TABLE [ETL].[Staging_FSE_ProductDiet](
	[Gtin] [char](14) NULL,
	[DietType] [varchar](25) NULL,
	[Value] [char](1) NULL
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
