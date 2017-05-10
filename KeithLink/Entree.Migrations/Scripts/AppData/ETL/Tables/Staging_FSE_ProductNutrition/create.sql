/****** Object:  Table [ETL].[Staging_FSE_ProductNutrition]    Script Date: 10/27/2016 1:05:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING OFF
GO
CREATE TABLE [ETL].[Staging_FSE_ProductNutrition](
	[Gtin] [char](14) NULL,
	[NutrientTypeCode] [varchar](100) NULL,
	[NutrientTypeDesc] [varchar](150) NULL,
	[MeasurmentTypeId] [varchar](5) NULL,
	[MeasurementValue] [decimal](20, 3) NULL,
	[DailyValue] [varchar](100) NULL
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
