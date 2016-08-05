CREATE TABLE [ETL].[Staging_KPay_Invoice] (
    [InvoiceNumber]  VARCHAR (30)   NOT NULL,
    [Division]       CHAR (5)       NOT NULL,
    [CustomerNumber] CHAR (6)       NOT NULL,
    [ItemSequence]   SMALLINT       NOT NULL,
    [InvoiceType]    CHAR (3)       NOT NULL,
    [InvoiceDate]    DATETIME       NOT NULL,
    [DueDate]        DATETIME       NOT NULL,
    [AmountDue]      DECIMAL (9, 2) NOT NULL,
    [DeleteFlag]     BIT            NOT NULL
);

