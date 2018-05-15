CREATE TABLE [Products].[PriceListCodes] (
	PriceListCode VARCHAR(5) NOT NULL PRIMARY KEY,
	PriceListCodeName VARCHAR(50) NOT NULL,
	ParentPriceListCode VARCHAR(5) NULL
)