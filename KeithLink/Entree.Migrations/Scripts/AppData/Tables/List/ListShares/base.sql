/****** Object:  Table [List].[ListShares]    Script Date: 10/27/2016 1:05:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [List].[ListShares](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[CustomerId] [nvarchar](max) NULL,
	[BranchId] [nvarchar](max) NULL,
	[CreatedUtc] [datetime] NOT NULL DEFAULT (getutcdate()),
	[ModifiedUtc] [datetime] NOT NULL DEFAULT (getutcdate()),
	[SharedList_Id] [bigint] NULL,
 CONSTRAINT [PK_List.ListShares] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Index [IX_SharedList_Id]    Script Date: 10/27/2016 1:05:25 PM ******/
CREATE NONCLUSTERED INDEX [IX_SharedList_Id] ON [List].[ListShares]
(
	[SharedList_Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
ALTER TABLE [List].[ListShares]  WITH CHECK ADD  CONSTRAINT [FK_List.ListShares_List.Lists_SharedList_Id] FOREIGN KEY([SharedList_Id])
REFERENCES [List].[Lists] ([Id])
GO
ALTER TABLE [List].[ListShares] CHECK CONSTRAINT [FK_List.ListShares_List.Lists_SharedList_Id]
GO
