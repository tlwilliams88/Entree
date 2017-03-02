CREATE TABLE [Orders].[Order2List] (
    [ControlNumber]     NVARCHAR (40)        NOT NULL,
    [ListId]            BIGINT          DEFAULT 0 NOT NULL,
    [CreatedUtc]        DATETIME        DEFAULT (getutcdate()) NOT NULL
    CONSTRAINT [PK_Orders.Order2List] PRIMARY KEY CLUSTERED ([ControlNumber] ASC)
);
GO
