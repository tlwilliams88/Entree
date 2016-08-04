CREATE TABLE [Configuration].[ExternalCatalogs] (
    [Id]               BIGINT        IDENTITY (1, 1) NOT NULL,
    [BekBranchId]      NVARCHAR (24) NULL,
    [ExternalBranchId] NVARCHAR (24) NULL,
    [Type]             INT           NOT NULL,
    [CreatedUtc]       DATETIME      DEFAULT (getutcdate()) NOT NULL,
    [ModifiedUtc]      DATETIME      DEFAULT (getutcdate()) NOT NULL,
    CONSTRAINT [PK_Configuration.ExternalCatalogs] PRIMARY KEY CLUSTERED ([Id] ASC)
);

