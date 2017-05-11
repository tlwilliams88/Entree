/****** Object:  Table [Configuration].[ExportSettings]    Script Date: 10/27/2016 1:05:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [Configuration].[ExportSettings](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[UserId] [uniqueidentifier] NOT NULL,
	[Type] [int] NOT NULL,
	[ListType] [int] NULL,
	[Settings] [nvarchar](max) NULL,
	[CreatedUtc] [datetime] NOT NULL DEFAULT (getutcdate()),
	[ModifiedUtc] [datetime] NOT NULL DEFAULT (getutcdate()),
	[ExportFormat] [nvarchar](max) NULL,
 CONSTRAINT [PK_Configuration.ExportSettings] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
