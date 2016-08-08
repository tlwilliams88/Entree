CREATE TABLE [ETL].[Staging_CustomerBid] (
    [Action]           VARCHAR (1)  NULL,
    [CompanyNumber]    VARCHAR (3)  NULL,
    [DivisionNumber]   VARCHAR (3)  NULL,
    [DepartmentNumber] VARCHAR (3)  NULL,
    [CustomerNumber]   VARCHAR (10) NULL,
    [PriorityNumber]   VARCHAR (3)  NULL,
    [BidNumber]        VARCHAR (10) NULL
);

GO

CREATE INDEX [IX_Stg_CustBid_BidNumver_DivisionNumber] ON [ETL].[Staging_CustomerBid] ([BidNumber], [DivisionNumber])