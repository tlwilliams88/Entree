ALTER TABLE [List].[ListItems]  WITH CHECK ADD  CONSTRAINT [FK_List.ListItems_List.Lists_List_Id] FOREIGN KEY([ParentList_Id])
REFERENCES [List].[Lists] ([Id])
GO
ALTER TABLE [List].[ListItems] CHECK CONSTRAINT [FK_List.ListItems_List.Lists_List_Id]
