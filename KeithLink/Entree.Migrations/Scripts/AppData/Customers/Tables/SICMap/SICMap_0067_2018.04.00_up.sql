CREATE TABLE [Customers].[SICMap](
    [CustomerNumber] VARCHAR(6) NOT NULL,
    [SIC] VARCHAR(4) NOT NULL,
    PRIMARY KEY (CustomerNumber, SIC)
)
GO
