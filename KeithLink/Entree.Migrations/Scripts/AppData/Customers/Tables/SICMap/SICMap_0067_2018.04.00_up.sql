CREATE TABLE [Customers].[SICMap](
    [BranchId] VARCHAR(3) NOT NULL,
    [CustomerNumber] VARCHAR(6) NOT NULL,
    [SIC] VARCHAR(4) NOT NULL,
    PRIMARY KEY (CustomerNumber, BranchId, SIC)
)
GO
