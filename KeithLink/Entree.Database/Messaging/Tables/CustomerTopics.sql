CREATE TABLE [Messaging].[CustomerTopics] (
    [Id]               BIGINT        IDENTITY (1, 1) NOT NULL,
    [CustomerNumber]   VARCHAR (9)   NULL,
    [ProviderTopicId]  VARCHAR (255) NULL,
    [NotificationType] INT           NOT NULL,
    [CreatedUtc]       DATETIME      DEFAULT (getutcdate()) NOT NULL,
    [ModifiedUtc]      DATETIME      DEFAULT (getutcdate()) NOT NULL,
    CONSTRAINT [PK_Messaging.CustomerTopics] PRIMARY KEY CLUSTERED ([Id] ASC)
);

