/****** Object:  Index [IX_ItemParent]    Script Date: 10/27/2016 1:05:25 PM ******/
CREATE NONCLUSTERED INDEX [IX_ItemParent] ON [List].[ListItems]
(
	[ItemNumber] ASC,
	[ParentList_Id] ASC,
	[Each] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 80) ON [PRIMARY]
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [ix_ListItems_ParentListId_Include]    Script Date: 10/27/2016 1:05:25 PM ******/
CREATE NONCLUSTERED INDEX [ix_ListItems_ParentListId_Include] ON [List].[ListItems]
(
	[ParentList_Id] ASC
)
INCLUDE ( 	[ItemNumber],
	[Label],
	[Par],
	[Note],
	[CreatedUtc],
	[ModifiedUtc],
	[Category],
	[Position],
	[FromDate],
	[ToDate],
	[Each],
	[Quantity],
	[CatalogId]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [IX_ParentId]    Script Date: 10/27/2016 1:05:25 PM ******/
CREATE NONCLUSTERED INDEX [IX_ParentId] ON [List].[ListItems]
(
	[ParentList_Id] ASC
)
INCLUDE ( 	[ItemNumber],
	[Category],
	[Position],
	[Each]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 80) ON [PRIMARY]
GO
/****** Object:  Index [IX_ParentList_Id]    Script Date: 10/27/2016 1:05:25 PM ******/
CREATE NONCLUSTERED INDEX [IX_ParentList_Id] ON [List].[ListItems]
(
	[ParentList_Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 80) ON [PRIMARY]
GO
SET ANSI_PADDING ON