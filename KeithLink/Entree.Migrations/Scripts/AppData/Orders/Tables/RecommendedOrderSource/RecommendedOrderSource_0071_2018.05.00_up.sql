

GO

CREATE TABLE [Orders].[RecommendedOrderSource] (
    [OrderSourceId]     INT             NOT NULL IDENTITY,
    [OrderSource]       NVARCHAR(20)    NOT NULL,
    [Description]       NVARCHAR(80)    NULL
    CONSTRAINT [PK_Orders.RecommendedOrderSource] PRIMARY KEY CLUSTERED ([OrderSource] ASC)
);
GO
