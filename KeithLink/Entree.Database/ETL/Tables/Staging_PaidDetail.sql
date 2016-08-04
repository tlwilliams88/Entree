CREATE TABLE [ETL].[Staging_PaidDetail] (
    [Action]           VARCHAR (50) NULL,
    [AdjustCode]       VARCHAR (50) NULL,
    [Company]          VARCHAR (50) NULL,
    [Division]         VARCHAR (50) NULL,
    [Department]       VARCHAR (3)  NULL,
    [Customer]         VARCHAR (10) NULL,
    [InvoiceNumber]    VARCHAR (20) NULL,
    [ReclineNumber]    VARCHAR (9)  NULL,
    [InvoiceType]      VARCHAR (1)  NULL,
    [DateOfLastOrder]  VARCHAR (8)  NULL,
    [InvoiceAmount]    VARCHAR (16) NULL,
    [CheckNumber]      VARCHAR (9)  NULL,
    [InvoiceReference] VARCHAR (20) NULL,
    [InvoiceDue]       VARCHAR (8)  NULL,
    [CombinedStmt]     VARCHAR (10) NULL
);

