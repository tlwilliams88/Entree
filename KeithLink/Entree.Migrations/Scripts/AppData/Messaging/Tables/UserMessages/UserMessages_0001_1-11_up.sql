/****** Object:  Table [Messaging].[UserMessages]    Script Date: 10/27/2016 1:05:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [Messaging].[UserMessages](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[CustomerNumber] [varchar](9) NULL,
	[UserId] [uniqueidentifier] NOT NULL,
	[NotificationType] [int] NOT NULL,
	[MessageReadUtc] [datetime] NULL,
	[CreatedUtc] [datetime] NOT NULL DEFAULT (getutcdate()),
	[ModifiedUtc] [datetime] NOT NULL DEFAULT (getutcdate()),
	[Body] [nvarchar](max) NULL,
	[Subject] [nvarchar](max) NULL,
	[Mandatory] [bit] NOT NULL DEFAULT ((0)),
	[Label] [nvarchar](max) NULL,
	[CustomerName] [nvarchar](250) NULL,
	[BranchId] [nvarchar](3) NULL,
 CONSTRAINT [PK_Messaging.UserMessages] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Index [idx_UserId]    Script Date: 10/27/2016 1:05:25 PM ******/
CREATE NONCLUSTERED INDEX [idx_UserId] ON [Messaging].[UserMessages]
(
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [idx_UserId_ReadDateUtc]    Script Date: 10/27/2016 1:05:25 PM ******/
CREATE NONCLUSTERED INDEX [idx_UserId_ReadDateUtc] ON [Messaging].[UserMessages]
(
	[UserId] ASC,
	[MessageReadUtc] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
