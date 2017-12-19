

GO

CREATE TABLE [Orders].[OrderedItemsFromList] (
    [ControlNumber]     NVARCHAR (40)   NOT NULL,
    [ItemNumber]        NVARCHAR (15)   NOT NULL,
    [SourceList]        NVARCHAR (80)   DEFAULT 0 NOT NULL,
    [CreatedUtc]        DATETIME        DEFAULT (getutcdate()) NOT NULL
    CONSTRAINT [PK_Orders.OrderedItemsFromList] PRIMARY KEY CLUSTERED ([ControlNumber] ASC, [ItemNumber] ASC)
);
GO
