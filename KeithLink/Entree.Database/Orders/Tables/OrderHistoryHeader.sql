CREATE TABLE [Orders].[OrderHistoryHeader] (
    [Id]                    BIGINT          IDENTITY (1, 1) NOT NULL,
    [OrderSystem]           CHAR (1)        NULL,
    [BranchId]              CHAR (3)        NULL,
    [CustomerNumber]        CHAR (6)        NULL,
    [InvoiceNumber]         VARCHAR (10)    NULL,
    [PONumber]              NVARCHAR (20)   NULL,
    [ControlNumber]         CHAR (7)        NULL,
    [OrderStatus]           CHAR (1)        NULL,
    [FutureItems]           BIT             NOT NULL,
    [ErrorStatus]           BIT             NOT NULL,
    [RouteNumber]           CHAR (4)        NULL,
    [CreatedUtc]            DATETIME        DEFAULT (getutcdate()) NOT NULL,
    [ModifiedUtc]           DATETIME        DEFAULT (getutcdate()) NOT NULL,
    [StopNumber]            CHAR (3)        NULL,
    [DeliveryOutOfSequence] BIT             NULL,
    [OriginalControlNumber] CHAR (7)        NULL,
    [IsSpecialOrder]        BIT             DEFAULT ((0)) NOT NULL,
    [RelatedControlNumber]  CHAR (7)        NULL,
    [DeliveryDate]          CHAR (10)       NULL,
    [ScheduledDeliveryTime] CHAR (19)       NULL,
    [EstimatedDeliveryTime] CHAR (19)       NULL,
    [ActualDeliveryTime]    CHAR (19)       NULL,
    [OrderSubtotal]         DECIMAL (18, 2) DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_Orders.OrderHistoryHeader] PRIMARY KEY CLUSTERED ([Id] ASC) WITH (FILLFACTOR = 80)
);


GO
CREATE NONCLUSTERED INDEX [ix_OrderHistoryheader_OrderSystem_includes]
    ON [Orders].[OrderHistoryHeader]([OrderSystem] ASC)
    INCLUDE([Id], [BranchId], [CustomerNumber], [InvoiceNumber], [PONumber], [ControlNumber], [OrderStatus], [FutureItems], [ErrorStatus], [RouteNumber], [CreatedUtc], [ModifiedUtc], [StopNumber], [DeliveryOutOfSequence], [OriginalControlNumber], [IsSpecialOrder], [RelatedControlNumber], [DeliveryDate], [ScheduledDeliveryTime], [EstimatedDeliveryTime], [ActualDeliveryTime], [OrderSubtotal]) WITH (FILLFACTOR = 100);


GO
CREATE NONCLUSTERED INDEX [IdxCustomerNumberByDate]
    ON [Orders].[OrderHistoryHeader]([CustomerNumber] ASC, [DeliveryDate] ASC) WITH (FILLFACTOR = 80);


GO
CREATE NONCLUSTERED INDEX [IdxOrderHeader]
    ON [Orders].[OrderHistoryHeader]([BranchId] ASC, [InvoiceNumber] ASC) WITH (FILLFACTOR = 80);

