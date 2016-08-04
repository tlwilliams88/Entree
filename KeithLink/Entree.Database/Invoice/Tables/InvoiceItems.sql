CREATE TABLE [Invoice].[InvoiceItems] (
    [Id]              BIGINT          IDENTITY (1, 1) NOT NULL,
    [QuantityOrdered] INT             NULL,
    [QuantityShipped] INT             NULL,
    [CatchWeightCode] BIT             NOT NULL,
    [ExtCatchWeight]  DECIMAL (18, 2) NULL,
    [ItemPrice]       DECIMAL (18, 2) NULL,
    [ExtSalesNet]     DECIMAL (18, 2) NULL,
    [CreatedUtc]      DATETIME        DEFAULT (getutcdate()) NOT NULL,
    [ModifiedUtc]     DATETIME        DEFAULT (getutcdate()) NOT NULL,
    [InvoiceId]       BIGINT          NOT NULL,
    [ItemNumber]      NVARCHAR (10)   NULL,
    [ClassCode]       CHAR (2)        NULL,
    [LineNumber]      NVARCHAR (6)    NULL,
    CONSTRAINT [PK_Invoice.InvoiceItems] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Invoice.InvoiceItems_Invoice.Invoices_InvoiceId] FOREIGN KEY ([InvoiceId]) REFERENCES [Invoice].[Invoices] ([Id]) ON DELETE CASCADE
);


GO
CREATE NONCLUSTERED INDEX [IX_InvoiceId]
    ON [Invoice].[InvoiceItems]([InvoiceId] ASC);

