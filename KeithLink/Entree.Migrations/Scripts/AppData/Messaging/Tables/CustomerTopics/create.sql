/****** Object:  Table [Messaging].[CustomerTopics]    Script Date: 10/27/2016 1:05:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [Messaging].[CustomerTopics](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[CustomerNumber] [varchar](9) NULL,
	[ProviderTopicId] [varchar](255) NULL,
	[NotificationType] [int] NOT NULL,
	[CreatedUtc] [datetime] NOT NULL,
	[ModifiedUtc] [datetime] NOT NULL,
 CONSTRAINT [PK_Messaging.CustomerTopics] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
ALTER TABLE [Messaging].[CustomerTopics] ADD  DEFAULT (getutcdate()) FOR [CreatedUtc]
GO
ALTER TABLE [Messaging].[CustomerTopics] ADD  DEFAULT (getutcdate()) FOR [ModifiedUtc]
GO
