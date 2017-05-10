/****** Object:  Table [Messaging].[UserTopicSubscriptions]    Script Date: 10/27/2016 1:05:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [Messaging].[UserTopicSubscriptions](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[UserId] [uniqueidentifier] NOT NULL,
	[ProviderSubscriptionId] [varchar](255) NULL,
	[NotificationEndpoint] [varchar](255) NULL,
	[CreatedUtc] [datetime] NOT NULL,
	[ModifiedUtc] [datetime] NOT NULL,
	[CustomerTopic_Id] [bigint] NULL,
	[Channel] [int] NOT NULL,
 CONSTRAINT [PK_Messaging.UserTopicSubscriptions] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Index [IX_CustomerTopic_Id]    Script Date: 10/27/2016 1:05:25 PM ******/
CREATE NONCLUSTERED INDEX [IX_CustomerTopic_Id] ON [Messaging].[UserTopicSubscriptions]
(
	[CustomerTopic_Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON

GO
ALTER TABLE [Messaging].[UserTopicSubscriptions] ADD  DEFAULT (getutcdate()) FOR [CreatedUtc]
GO
ALTER TABLE [Messaging].[UserTopicSubscriptions] ADD  DEFAULT (getutcdate()) FOR [ModifiedUtc]
GO
ALTER TABLE [Messaging].[UserTopicSubscriptions] ADD  DEFAULT ((0)) FOR [Channel]
GO
ALTER TABLE [Messaging].[UserTopicSubscriptions]  WITH CHECK ADD  CONSTRAINT [FK_Messaging.UserTopicSubscriptions_Messaging.CustomerTopics_CustomerTopic_Id] FOREIGN KEY([CustomerTopic_Id])
REFERENCES [Messaging].[CustomerTopics] ([Id])
GO
ALTER TABLE [Messaging].[UserTopicSubscriptions] CHECK CONSTRAINT [FK_Messaging.UserTopicSubscriptions_Messaging.CustomerTopics_CustomerTopic_Id]
GO
