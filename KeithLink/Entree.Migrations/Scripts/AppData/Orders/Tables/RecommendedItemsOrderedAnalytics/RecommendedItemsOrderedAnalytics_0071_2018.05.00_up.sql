

GO

CREATE TABLE [Orders].[RecommendedItemsOrderedAnalytics] (
    [ControlNumber]     [char](7)       NOT NULL,
	[ItemNumber]        [char](6)       NOT NULL,
	[UnitOfMeasure]     [char](1)       NULL,
    [OrderSourceId]     INT             NOT NULL,
    [CreatedUtc]        DATETIME        DEFAULT (getutcdate()) NOT NULL
    CONSTRAINT [PK_Orders.RecommendedItemsOrderedAnalytics] PRIMARY KEY CLUSTERED ([ControlNumber] ASC, [ItemNumber] ASC)
);
GO
