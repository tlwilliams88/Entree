CREATE TABLE [Orders].[UserActiveCarts] (
    [Id]          BIGINT           IDENTITY (1, 1) NOT NULL,
    [UserId]      UNIQUEIDENTIFIER NOT NULL,
    [CartId]      UNIQUEIDENTIFIER NOT NULL,
    [CreatedUtc]  DATETIME         DEFAULT (getutcdate()) NOT NULL,
    [ModifiedUtc] DATETIME         DEFAULT (getutcdate()) NOT NULL,
    [CustomerId]  NVARCHAR (MAX)   NULL,
    [BranchId]    NVARCHAR (MAX)   NULL,
    CONSTRAINT [PK_Orders.UserActiveCarts] PRIMARY KEY CLUSTERED ([Id] ASC)
);

