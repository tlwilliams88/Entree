CREATE TABLE [Customers].[CustomerOptions](
    [BranchId] VARCHAR(3) NOT NULL,
    [CustomerNumber] VARCHAR(6) NOT NULL,
    [Option] VARCHAR(20) NOT NULL,
	[Value] DECIMAL(10,2) NOT NULL,
    PRIMARY KEY (BranchId, CustomerNumber)
)
GO