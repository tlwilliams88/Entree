/****** Object:  Table [dbo].[AuditLog]    Script Date: 10/27/2016 1:05:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AuditLog](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[Type] [int] NOT NULL,
	[TypeDescription] [nvarchar](50) NULL,
	[Actor] [nvarchar](100) NULL,
	[Information] [nvarchar](max) NULL,
	[CreatedUtc] [datetime] NOT NULL DEFAULT (getutcdate()),
 CONSTRAINT [PK_Auditing.AuditRecords] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
