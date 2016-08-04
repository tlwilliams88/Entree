CREATE TABLE [dbo].[AuditLog] (
    [Id]              BIGINT         IDENTITY (1, 1) NOT NULL,
    [Type]            INT            NOT NULL,
    [TypeDescription] NVARCHAR (50)  NULL,
    [Actor]           NVARCHAR (100) NULL,
    [Information]     NVARCHAR (MAX) NULL,
    [CreatedUtc]      DATETIME       DEFAULT (getutcdate()) NOT NULL,
    CONSTRAINT [PK_Auditing.AuditRecords] PRIMARY KEY CLUSTERED ([Id] ASC)
);

