/****** Object:  Table [List].[ListItems]    Script Date: 10/27/2016 1:05:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [List].[ListItems](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[ItemNumber] [nvarchar](15) NOT NULL,
	[Label] [nvarchar](150) NULL,
	[Par] [decimal](18, 2) NOT NULL,
	[Note] [nvarchar](200) NULL,
	[CreatedUtc] [datetime] NOT NULL DEFAULT (getutcdate()),
	[ParentList_Id] [bigint] NULL,
	[ModifiedUtc] [datetime] NOT NULL DEFAULT (getutcdate()),
	[Category] [nvarchar](40) NULL,
	[Position] [int] NOT NULL DEFAULT ((0)),
	[FromDate] [datetime] NULL,
	[ToDate] [datetime] NULL,
	[Each] [bit] NULL,
	[Quantity] [decimal](18, 2) NOT NULL DEFAULT ((0)),
	[CatalogId] [nvarchar](24) NULL,
 CONSTRAINT [PK_List.ListItems] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 80) ON [PRIMARY]
) ON [PRIMARY]

GO
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

GO
ALTER TABLE [List].[ListItems]  WITH CHECK ADD  CONSTRAINT [FK_List.ListItems_List.Lists_List_Id] FOREIGN KEY([ParentList_Id])
REFERENCES [List].[Lists] ([Id])
GO
ALTER TABLE [List].[ListItems] CHECK CONSTRAINT [FK_List.ListItems_List.Lists_List_Id]
GO
