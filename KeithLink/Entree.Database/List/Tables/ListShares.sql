CREATE TABLE [List].[ListShares] (
    [Id]            BIGINT         IDENTITY (1, 1) NOT NULL,
    [CustomerId]    NVARCHAR (MAX) NULL,
    [BranchId]      NVARCHAR (MAX) NULL,
    [CreatedUtc]    DATETIME       DEFAULT (getutcdate()) NOT NULL,
    [ModifiedUtc]   DATETIME       DEFAULT (getutcdate()) NOT NULL,
    [SharedList_Id] BIGINT         NULL,
    CONSTRAINT [PK_List.ListShares] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_List.ListShares_List.Lists_SharedList_Id] FOREIGN KEY ([SharedList_Id]) REFERENCES [List].[Lists] ([Id])
);


GO
CREATE NONCLUSTERED INDEX [IX_SharedList_Id]
    ON [List].[ListShares]([SharedList_Id] ASC);

