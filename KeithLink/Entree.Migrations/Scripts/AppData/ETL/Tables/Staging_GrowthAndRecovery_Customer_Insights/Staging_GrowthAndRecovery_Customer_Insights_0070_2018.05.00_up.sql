CREATE TABLE [ETL].[Staging_GrowthAndRecovery_Customer_Insights](
    [Name] VARCHAR(100) NOT NULL,
    [CurrencyIsoCode] NVARCHAR(10) NULL,
    [CustomerInsightKey] INT NOT NULL,
    [CustomerInsightVersionKey] INT NULL,
    [CustomerId] VARCHAR(100) NOT NULL,
    [CustomerCity] VARCHAR(100) NULL,
    [CustomerState] VARCHAR(100) NULL,
    [CustomerCountry] VARCHAR(5) NULL,
    [StatusSummary] VARCHAR(100) NULL,
    [CustomerInsightStatus] VARCHAR(100) NULL,
    [FlaggedForInitialDelivery] BIT NOT NULL,
    [InitiativeName] VARCHAR(100) NULL,
    [ReportingGroup] VARCHAR(100) NULL,
    [CustomerInsightType] VARCHAR(5) NULL
) ON [PRIMARY]
