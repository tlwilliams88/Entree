ALTER TABLE [List].[ListShares]  WITH CHECK ADD  CONSTRAINT [FK_List.ListShares_List.Lists_SharedList_Id] FOREIGN KEY([SharedList_Id])
REFERENCES [List].[Lists] ([Id])
GO
ALTER TABLE [List].[ListShares] CHECK CONSTRAINT [FK_List.ListShares_List.Lists_SharedList_Id]
GO
