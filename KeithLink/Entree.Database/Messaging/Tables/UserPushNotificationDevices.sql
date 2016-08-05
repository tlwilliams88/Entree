CREATE TABLE [Messaging].[UserPushNotificationDevices] (
    [Id]                 BIGINT           IDENTITY (1, 1) NOT NULL,
    [UserId]             UNIQUEIDENTIFIER NOT NULL,
    [DeviceId]           NVARCHAR (255)   NOT NULL,
    [ProviderToken]      NVARCHAR (255)   NOT NULL,
    [ProviderEndpointId] NVARCHAR (255)   NULL,
    [DeviceOS]           INT              NOT NULL,
    [CreatedUtc]         DATETIME         DEFAULT (getutcdate()) NOT NULL,
    [ModifiedUtc]        DATETIME         DEFAULT (getutcdate()) NOT NULL,
    [Enabled]            BIT              NULL,
    CONSTRAINT [PK_Messaging.UserPushNotificationDevices] PRIMARY KEY CLUSTERED ([Id] ASC)
);


GO
CREATE NONCLUSTERED INDEX [IX_UserId]
    ON [Messaging].[UserPushNotificationDevices]([UserId] ASC);

