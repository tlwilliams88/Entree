CREATE TABLE [Orders].[RecommendedItemsOrderedAnalytics] (
    [CartId]            VARCHAR (40)      NOT NULL,
    [ControlNumber]     VARCHAR (7)       NULL,
	[ItemNumber]        VARCHAR (6)       NOT NULL,
	[UnitOfMeasure]     VARCHAR (1)       NULL,
    [OrderSourceId]     INT             NOT NULL,
    [CreatedUtc]        DATETIME        DEFAULT (getutcdate()) NOT NULL
    CONSTRAINT [PK_Orders.RecommendedItemsOrderedAnalytics] PRIMARY KEY CLUSTERED ([CartId] ASC, [ItemNumber] ASC)
);
