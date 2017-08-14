

GO

CREATE TABLE [Orders].[OrderedFromList] (
    [ControlNumber]     NVARCHAR (40)        NOT NULL,
    [ListId]            BIGINT          DEFAULT 0 NOT NULL,
    [CreatedUtc]        DATETIME        DEFAULT (getutcdate()) NOT NULL
    CONSTRAINT [PK_Orders.OrderedFromList] PRIMARY KEY CLUSTERED ([ControlNumber] ASC)
);
GO
