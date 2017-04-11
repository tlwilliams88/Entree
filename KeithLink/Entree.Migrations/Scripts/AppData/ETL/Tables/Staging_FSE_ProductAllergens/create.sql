/****** Object:  Table [ETL].[Staging_FSE_ProductAllergens]    Script Date: 10/27/2016 1:05:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING OFF
GO
CREATE TABLE [ETL].[Staging_FSE_ProductAllergens](
	[Gtin] [char](14) NULL
) ON [PRIMARY]
SET ANSI_PADDING ON
ALTER TABLE [ETL].[Staging_FSE_ProductAllergens] ADD [AllergenTypeCode] [varchar](10) NULL
SET ANSI_PADDING OFF
ALTER TABLE [ETL].[Staging_FSE_ProductAllergens] ADD [AllergenTypeDesc] [varchar](50) NULL
ALTER TABLE [ETL].[Staging_FSE_ProductAllergens] ADD [LevelOfContainment] [varchar](20) NULL

GO
SET ANSI_PADDING OFF
GO
