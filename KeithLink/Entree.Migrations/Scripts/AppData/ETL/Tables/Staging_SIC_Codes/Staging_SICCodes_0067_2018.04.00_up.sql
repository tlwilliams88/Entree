SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [ETL].[Staging_SIC_Codes](
    [SIC_Code] VARCHAR(4) NOT NULL,
    [Description] VARCHAR(80) NOT NULL
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
