CREATE TABLE [ETL].[Staging_WorksheetItems] (
    [Action]           VARCHAR (1)  NULL,
    [CompanyNumber]    VARCHAR (3)  NULL,
    [DivisionNumber]   VARCHAR (3)  NULL,
    [DepartmentNumber] VARCHAR (3)  NULL,
    [CustomerNumber]   VARCHAR (10) NULL,
    [ItemNumber]       VARCHAR (10) NULL,
    [BrokenCaseCode]   VARCHAR (1)  NULL,
    [ItemPrice]        VARCHAR (10) NULL,
    [QtyOrdered]       VARCHAR (7)  NULL,
    [DateOfLastOrder]  VARCHAR (8)  NULL
);


GO
CREATE NONCLUSTERED INDEX [IX_CustDiv]
    ON [ETL].[Staging_WorksheetItems]([DivisionNumber] ASC, [CustomerNumber] ASC)
    INCLUDE([ItemNumber], [BrokenCaseCode]);

