CREATE TABLE [Configuration].[MessageTemplates] (
    [Id]          BIGINT         IDENTITY (1, 1) NOT NULL,
    [TemplateKey] NVARCHAR (50)  NULL,
    [Subject]     NVARCHAR (MAX) NULL,
    [IsBodyHtml]  BIT            NOT NULL,
    [Body]        NVARCHAR (MAX) NULL,
    [CreatedUtc]  DATETIME       DEFAULT (getutcdate()) NOT NULL,
    [ModifiedUtc] DATETIME       DEFAULT (getutcdate()) NOT NULL,
    [Type]        INT            DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_Configuration.EmailTemplates] PRIMARY KEY CLUSTERED ([Id] ASC)
);


GO
CREATE NONCLUSTERED INDEX [IX_TemplateKey]
    ON [Configuration].[MessageTemplates]([TemplateKey] ASC);

