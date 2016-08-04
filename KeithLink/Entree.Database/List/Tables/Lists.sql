CREATE TABLE [List].[Lists] (
    [Id]            BIGINT           IDENTITY (1, 1) NOT NULL,
    [UserId]        UNIQUEIDENTIFIER NULL,
    [DisplayName]   NVARCHAR (MAX)   NULL,
    [Type]          INT              NOT NULL,
    [CustomerId]    NVARCHAR (10)    NULL,
    [BranchId]      NVARCHAR (10)    NULL,
    [AccountNumber] NVARCHAR (MAX)   NULL,
    [ReadOnly]      BIT              NOT NULL,
    [CreatedUtc]    DATETIME         DEFAULT (getutcdate()) NOT NULL,
    [ModifiedUtc]   DATETIME         DEFAULT (getutcdate()) NOT NULL,
    CONSTRAINT [PK_List.Lists] PRIMARY KEY CLUSTERED ([Id] ASC)
);


GO
CREATE NONCLUSTERED INDEX [IX_CustBranch]
    ON [List].[Lists]([CustomerId] ASC, [BranchId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_Type]
    ON [List].[Lists]([Type] ASC);

