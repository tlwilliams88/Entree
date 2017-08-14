ALTER TABLE [Orders].[OrderHistoryDetail]  WITH CHECK ADD  CONSTRAINT [FK_Orders.OrderHistoryDetail_Orders.OrderHistoryHeader_OrderHistoryHeader_Id] FOREIGN KEY([OrderHistoryHeader_Id])
REFERENCES [Orders].[OrderHistoryHeader] ([Id])
GO
ALTER TABLE [Orders].[OrderHistoryDetail] CHECK CONSTRAINT [FK_Orders.OrderHistoryDetail_Orders.OrderHistoryHeader_OrderHistoryHeader_Id]
GO