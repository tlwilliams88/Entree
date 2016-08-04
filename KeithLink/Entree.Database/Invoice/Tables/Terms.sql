CREATE TABLE [Invoice].[Terms] (
    [Id]          BIGINT        IDENTITY (1, 1) NOT NULL,
    [BranchId]    NVARCHAR (3)  NULL,
    [TermCode]    INT           NOT NULL,
    [Description] NVARCHAR (25) NULL,
    [Age1]        INT           NOT NULL,
    [Age2]        INT           NOT NULL,
    [Age3]        INT           NOT NULL,
    [Age4]        INT           NOT NULL,
    [CreatedUtc]  DATETIME      DEFAULT (getutcdate()) NOT NULL,
    [ModifiedUtc] DATETIME      DEFAULT (getutcdate()) NOT NULL,
    CONSTRAINT [PK_Invoice.Terms] PRIMARY KEY CLUSTERED ([Id] ASC)
);

