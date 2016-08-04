CREATE TABLE [Configuration].[ExportSettings] (
    [Id]           BIGINT           IDENTITY (1, 1) NOT NULL,
    [UserId]       UNIQUEIDENTIFIER NOT NULL,
    [Type]         INT              NOT NULL,
    [ListType]     INT              NULL,
    [Settings]     NVARCHAR (MAX)   NULL,
    [CreatedUtc]   DATETIME         DEFAULT (getutcdate()) NOT NULL,
    [ModifiedUtc]  DATETIME         DEFAULT (getutcdate()) NOT NULL,
    [ExportFormat] NVARCHAR (MAX)   NULL,
    CONSTRAINT [PK_Configuration.ExportSettings] PRIMARY KEY CLUSTERED ([Id] ASC)
);

