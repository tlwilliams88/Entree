CREATE TABLE [ETL].[Staging_GrowthAndRecovery_Product_Grouping_Insights](
    [CurrencyIsoCode] NVARCHAR(50) NULL,
    [ProductGroupingInsightKey] INT NOT NULL,
    [Amount] DECIMAL(20,10) NULL,
    [CustomerInsight] INT NOT NULL,
    [State] NVARCHAR(50) NULL,
    [ProductGrouping] INT NOT NULL,
    [RunDate] NVARCHAR(50) NULL,
    [ProductGroupingInsightType] INT NULL,
    [CurrentPrice] NVARCHAR(50) NULL,
    [RecommendedPrice] NVARCHAR(50) NULL,
    [SuperGroup] NVARCHAR(50) NULL,
    [HistoricSales] DECIMAL(20,10) NULL,
    [RecentSales] DECIMAL(20,10) NULL,
    [ProfilePeriodSales] DECIMAL(20,10) NULL
) ON [PRIMARY]
