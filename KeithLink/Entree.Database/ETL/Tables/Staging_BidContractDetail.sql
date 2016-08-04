CREATE TABLE [ETL].[Staging_BidContractDetail] (
    [Action]              VARCHAR (1)  NULL,
    [CompanyNumber]       VARCHAR (3)  NULL,
    [DivisionNumber]      VARCHAR (3)  NULL,
    [DepartmentNumber]    VARCHAR (3)  NULL,
    [BidNumber]           VARCHAR (10) NULL,
    [ItemNumber]          VARCHAR (10) NULL,
    [BidLineNumber]       VARCHAR (5)  NULL,
    [CategoryNumber]      VARCHAR (5)  NULL,
    [CategoryDescription] VARCHAR (40) NULL,
    [ForceEachOrCaseOnly] VARCHAR (1)  NULL
);


GO
CREATE NONCLUSTERED INDEX [ix_INBFCF]
    ON [ETL].[Staging_BidContractDetail]([DivisionNumber] ASC, [BidNumber] ASC)
    INCLUDE([ItemNumber], [BidLineNumber], [CategoryDescription], [ForceEachOrCaseOnly]);

