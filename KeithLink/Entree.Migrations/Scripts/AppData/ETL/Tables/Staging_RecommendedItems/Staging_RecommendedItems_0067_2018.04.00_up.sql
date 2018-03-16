SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [ETL].[Staging_RecommendedItems](
    [ItemNumber] VARCHAR(6) NOT NULL,
    [RecommendedItemNumber] VARCHAR(6) NOT NULL,
    [Support] VARCHAR(25) NOT NULL,
    [Confidence] DECIMAL(10,9) NOT NULL,
    [Context_Attribute] VARCHAR(100) NOT NULL,
    [Primary_PriceListCode] VARCHAR(2) NOT NULL,
    [Primary_SubPriceListCode] VARCHAR(2) NOT NULL,
    [Secondary_PriceListCode] VARCHAR(2) NOT NULL,
    [Secondary_SubPriceListCode] VARCHAR(2) NOT NULL
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
