CREATE TABLE [Orders].[OrderHistoryDetail] (
    [Id]                         BIGINT          IDENTITY (1, 1) NOT NULL,
    [ItemNumber]                 CHAR (6)        NULL,
    [LineNumber]                 INT             NOT NULL,
    [OrderQuantity]              INT             NOT NULL,
    [ShippedQuantity]            INT             NOT NULL,
    [UnitOfMeasure]              CHAR (1)        NULL,
    [CatchWeight]                BIT             NOT NULL,
    [ItemDeleted]                BIT             NOT NULL,
    [SubbedOriginalItemNumber]   CHAR (6)        NULL,
    [ReplacedOriginalItemNumber] CHAR (6)        NULL,
    [ItemStatus]                 CHAR (1)        NULL,
    [TotalShippedWeight]         DECIMAL (18, 2) NOT NULL,
    [CreatedUtc]                 DATETIME        DEFAULT (getutcdate()) NOT NULL,
    [ModifiedUtc]                DATETIME        DEFAULT (getutcdate()) NOT NULL,
    [OrderHistoryHeader_Id]      BIGINT          NULL,
    [BranchId]                   CHAR (3)        NULL,
    [InvoiceNumber]              VARCHAR (10)    NULL,
    [SellPrice]                  DECIMAL (18, 2) DEFAULT ((0)) NOT NULL,
    [Source]                     CHAR (3)        NULL,
    [ManufacturerId]             NVARCHAR (25)   NULL,
    [SpecialOrderHeaderId]       CHAR (7)        NULL,
    [SpecialOrderLineNumber]     CHAR (3)        NULL,
    CONSTRAINT [PK_Orders.OrderHistoryDetail] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Orders.OrderHistoryDetail_Orders.OrderHistoryHeader_OrderHistoryHeader_Id] FOREIGN KEY ([OrderHistoryHeader_Id]) REFERENCES [Orders].[OrderHistoryHeader] ([Id])
);


GO
CREATE NONCLUSTERED INDEX [IdxItemUsageGrouping]
    ON [Orders].[OrderHistoryDetail]([ItemNumber] ASC, [UnitOfMeasure] ASC) WITH (FILLFACTOR = 80);


GO
CREATE NONCLUSTERED INDEX [IdxOrderDetail]
    ON [Orders].[OrderHistoryDetail]([BranchId] ASC, [InvoiceNumber] ASC, [LineNumber] ASC) WITH (FILLFACTOR = 80);


GO
CREATE NONCLUSTERED INDEX [IX_OrderHistoryHeader_Id]
    ON [Orders].[OrderHistoryDetail]([OrderHistoryHeader_Id] ASC) WITH (FILLFACTOR = 80);

