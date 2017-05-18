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
