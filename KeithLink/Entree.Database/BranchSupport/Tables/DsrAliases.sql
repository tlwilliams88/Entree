CREATE TABLE [BranchSupport].[DsrAliases] (
    [Id]          BIGINT           IDENTITY (1, 1) NOT NULL,
    [UserId]      UNIQUEIDENTIFIER NOT NULL,
    [UserName]    VARCHAR (200)    NOT NULL,
    [BranchId]    CHAR (3)         NOT NULL,
    [CreatedUtc]  DATETIME         DEFAULT (getutcdate()) NOT NULL,
    [ModifiedUtc] DATETIME         DEFAULT (getutcdate()) NOT NULL,
    [DsrNumber]   CHAR (6)         DEFAULT ('') NOT NULL,
    CONSTRAINT [PK_BranchSupport.DsrAliases] PRIMARY KEY CLUSTERED ([Id] ASC)
);


GO
CREATE NONCLUSTERED INDEX [IX_UserId]
    ON [BranchSupport].[DsrAliases]([UserId] ASC);

