/****** Object:  Table [BranchSupport].[BranchSupports]    Script Date: 10/27/2016 1:05:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [BranchSupport].[BranchSupports](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[BranchName] [nvarchar](max) NULL,
	[BranchId] [nvarchar](max) NULL,
	[SupportPhoneNumber] [nvarchar](max) NULL,
	[TollFreeNumber] [nvarchar](max) NULL,
	[Email] [nvarchar](max) NULL,
	[CreatedUtc] [datetime] NOT NULL,
	[ModifiedUtc] [datetime] NOT NULL,
 CONSTRAINT [PK_BranchSupport.BranchSupports] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
ALTER TABLE [BranchSupport].[BranchSupports] ADD  DEFAULT (getutcdate()) FOR [CreatedUtc]
GO
ALTER TABLE [BranchSupport].[BranchSupports] ADD  DEFAULT (getutcdate()) FOR [ModifiedUtc]
GO
