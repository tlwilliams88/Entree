CREATE TABLE [BranchSupport].[BranchSupports] (
    [Id]                 BIGINT         IDENTITY (1, 1) NOT NULL,
    [BranchName]         NVARCHAR (MAX) NULL,
    [BranchId]           NVARCHAR (MAX) NULL,
    [SupportPhoneNumber] NVARCHAR (MAX) NULL,
    [TollFreeNumber]     NVARCHAR (MAX) NULL,
    [Email]              NVARCHAR (MAX) NULL,
    [CreatedUtc]         DATETIME       DEFAULT (getutcdate()) NOT NULL,
    [ModifiedUtc]        DATETIME       DEFAULT (getutcdate()) NOT NULL,
    CONSTRAINT [PK_BranchSupport.BranchSupports] PRIMARY KEY CLUSTERED ([Id] ASC)
);

