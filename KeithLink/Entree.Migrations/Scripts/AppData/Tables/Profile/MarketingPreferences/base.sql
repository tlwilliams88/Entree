/****** Object:  Table [Profile].[MarketingPreferences]    Script Date: 10/27/2016 1:05:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [Profile].[MarketingPreferences](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[Email] [nvarchar](150) NULL,
	[BranchId] [nvarchar](max) NULL,
	[CurrentCustomer] [bit] NOT NULL,
	[LearnMore] [bit] NOT NULL,
	[RegisteredOn] [datetime] NOT NULL,
	[CreatedUtc] [datetime] NOT NULL,
	[ModifiedUtc] [datetime] NOT NULL,
 CONSTRAINT [PK_Profile.MarketingPreferences] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
ALTER TABLE [Profile].[MarketingPreferences] ADD  DEFAULT (getutcdate()) FOR [CreatedUtc]
GO
ALTER TABLE [Profile].[MarketingPreferences] ADD  DEFAULT (getutcdate()) FOR [ModifiedUtc]
GO
