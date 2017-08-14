ALTER TABLE [Messaging].[UserTopicSubscriptions]  WITH CHECK ADD  CONSTRAINT [FK_Messaging.UserTopicSubscriptions_Messaging.CustomerTopics_CustomerTopic_Id] FOREIGN KEY([CustomerTopic_Id])
REFERENCES [Messaging].[CustomerTopics] ([Id])
GO
ALTER TABLE [Messaging].[UserTopicSubscriptions] CHECK CONSTRAINT [FK_Messaging.UserTopicSubscriptions_Messaging.CustomerTopics_CustomerTopic_Id]
GO
