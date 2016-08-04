CREATE TABLE [Messaging].[UserTopicSubscriptions] (
    [Id]                     BIGINT           IDENTITY (1, 1) NOT NULL,
    [UserId]                 UNIQUEIDENTIFIER NOT NULL,
    [ProviderSubscriptionId] VARCHAR (255)    NULL,
    [NotificationEndpoint]   VARCHAR (255)    NULL,
    [CreatedUtc]             DATETIME         DEFAULT (getutcdate()) NOT NULL,
    [ModifiedUtc]            DATETIME         DEFAULT (getutcdate()) NOT NULL,
    [CustomerTopic_Id]       BIGINT           NULL,
    [Channel]                INT              DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_Messaging.UserTopicSubscriptions] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Messaging.UserTopicSubscriptions_Messaging.CustomerTopics_CustomerTopic_Id] FOREIGN KEY ([CustomerTopic_Id]) REFERENCES [Messaging].[CustomerTopics] ([Id])
);


GO
CREATE NONCLUSTERED INDEX [IX_CustomerTopic_Id]
    ON [Messaging].[UserTopicSubscriptions]([CustomerTopic_Id] ASC);

