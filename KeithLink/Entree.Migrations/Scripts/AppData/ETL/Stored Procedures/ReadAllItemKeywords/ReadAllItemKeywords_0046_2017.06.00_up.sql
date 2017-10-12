CREATE PROC [ETL].[ReadAllItemKeywords]
AS
SELECT ItemNumber, Keywords FROM [ETL].[Staging_ItemKeywords]
GROUP BY ItemNumber, Keywords