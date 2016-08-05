CREATE TABLE [Invoice].[Invoices] (
    [Id]             BIGINT          IDENTITY (1, 1) NOT NULL,
    [InvoiceNumber]  NVARCHAR (MAX)  NULL,
    [OrderDate]      DATETIME        NULL,
    [CustomerNumber] NVARCHAR (MAX)  NULL,
    [CreatedUtc]     DATETIME        DEFAULT (getutcdate()) NOT NULL,
    [ModifiedUtc]    DATETIME        DEFAULT (getutcdate()) NOT NULL,
    [BranchId]       NVARCHAR (3)    NULL,
    [InvoiceDate]    DATETIME        NULL,
    [Type]           INT             DEFAULT ((0)) NOT NULL,
    [Amount]         DECIMAL (18, 2) DEFAULT ((0)) NOT NULL,
    [Status]         INT             DEFAULT ((0)) NOT NULL,
    [DueDate]        DATETIME        NULL,
    CONSTRAINT [PK_Invoice.Invoices] PRIMARY KEY CLUSTERED ([Id] ASC)
);

