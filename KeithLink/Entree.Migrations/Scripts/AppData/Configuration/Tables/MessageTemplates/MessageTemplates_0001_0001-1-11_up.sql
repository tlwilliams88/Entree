/****** Object:  Table [Configuration].[MessageTemplates]    Script Date: 10/27/2016 1:05:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [Configuration].[MessageTemplates](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[TemplateKey] [nvarchar](50) NULL,
	[Subject] [nvarchar](max) NULL,
	[IsBodyHtml] [bit] NOT NULL,
	[Body] [nvarchar](max) NULL,
	[CreatedUtc] [datetime] NOT NULL DEFAULT (getutcdate()),
	[ModifiedUtc] [datetime] NOT NULL DEFAULT (getutcdate()),
	[Type] [int] NOT NULL DEFAULT ((0)),
 CONSTRAINT [PK_Configuration.EmailTemplates] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Index [IX_TemplateKey]    Script Date: 10/27/2016 1:05:25 PM ******/
CREATE NONCLUSTERED INDEX [IX_TemplateKey] ON [Configuration].[MessageTemplates]
(
	[TemplateKey] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON

GO
