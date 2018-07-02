USE [BEK_Commerce_AppData]
GO

/****** Object:  Table [Messaging].[UserFeedback]    Script Date: 6/22/2018 5:10:09 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [Messaging].[UserFeedback](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[UserId] [uniqueidentifier] NOT NULL,
	[UserFirstName] [varchar](250) NULL,
	[UserLastName] [varchar](250) NULL,
	[BranchId] [varchar](50) NULL,
	[CustomerNumber] [varchar](50) NULL,
	[CustomerName] [varchar](250) NULL,
	[SalesRepName] [varchar](250) NULL,
	[Audience] [varchar](50) NULL,
	[SourceName] [varchar](250) NULL,
	[TargetName] [varchar](250) NULL,
	[SourceEmailAddress] [varchar](250) NULL,
	[TargetEmailAddress] [varchar](250) NULL,
	[Subject] [varchar](250) NULL,
	[Content] [varchar](max) NULL,
	[BrowserUserAgent] [varchar](250) NULL,
	[BrowserVendor] [varchar](250) NULL,
	[CreatedUtc] [datetime] NOT NULL DEFAULT (getutcdate()),
 CONSTRAINT [PK_Messaging.UserFeedback] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO


