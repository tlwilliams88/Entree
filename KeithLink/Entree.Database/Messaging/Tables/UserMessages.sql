CREATE TABLE [Messaging].[UserMessages] (
    [Id]               BIGINT           IDENTITY (1, 1) NOT NULL,
    [CustomerNumber]   VARCHAR (9)      NULL,
    [UserId]           UNIQUEIDENTIFIER NOT NULL,
    [NotificationType] INT              NOT NULL,
    [MessageReadUtc]   DATETIME         NULL,
    [CreatedUtc]       DATETIME         DEFAULT (getutcdate()) NOT NULL,
    [ModifiedUtc]      DATETIME         DEFAULT (getutcdate()) NOT NULL,
    [Body]             NVARCHAR (MAX)   NULL,
    [Subject]          NVARCHAR (MAX)   NULL,
    [Mandatory]        BIT              DEFAULT ((0)) NOT NULL,
    [Label]            NVARCHAR (MAX)   NULL,
    [CustomerName]     NVARCHAR (250)   NULL,
    [BranchId]         NVARCHAR (3)     NULL,
    CONSTRAINT [PK_Messaging.UserMessages] PRIMARY KEY CLUSTERED ([Id] ASC)
);


GO
CREATE NONCLUSTERED INDEX [idx_UserId]
    ON [Messaging].[UserMessages]([UserId] ASC);


GO
CREATE NONCLUSTERED INDEX [idx_UserId_ReadDateUtc]
    ON [Messaging].[UserMessages]([UserId] ASC, [MessageReadUtc] ASC);

