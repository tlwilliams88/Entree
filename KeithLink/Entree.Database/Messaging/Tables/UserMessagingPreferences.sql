CREATE TABLE [Messaging].[UserMessagingPreferences] (
    [Id]               BIGINT           IDENTITY (1, 1) NOT NULL,
    [UserId]           UNIQUEIDENTIFIER NOT NULL,
    [NotificationType] INT              NOT NULL,
    [Channel]          INT              NOT NULL,
    [CustomerNumber]   VARCHAR (9)      NULL,
    [CreatedUtc]       DATETIME         DEFAULT (getutcdate()) NOT NULL,
    [ModifiedUtc]      DATETIME         DEFAULT (getutcdate()) NOT NULL,
    [BranchId]         VARCHAR (4)      NULL,
    CONSTRAINT [PK_Messaging.UserMessagingPreferences] PRIMARY KEY CLUSTERED ([Id] ASC)
);

