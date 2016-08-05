CREATE TABLE [ETL].[Staging_FSE_ProductNutrition] (
    [Gtin]             CHAR (14)       NULL,
    [NutrientTypeCode] VARCHAR (100)   NULL,
    [NutrientTypeDesc] VARCHAR (150)   NULL,
    [MeasurmentTypeId] VARCHAR (5)     NULL,
    [MeasurementValue] DECIMAL (20, 3) NULL,
    [DailyValue]       VARCHAR (100)   NULL
);

