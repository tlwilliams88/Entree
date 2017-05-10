/****** Object:  Table [BranchSupport].[DsrAliases]    Script Date: 10/27/2016 1:05:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [BranchSupport].[DsrAliases](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[UserId] [uniqueidentifier] NOT NULL,
	[UserName] [varchar](200) NOT NULL,
	[BranchId] [char](3) NOT NULL,
	[CreatedUtc] [datetime] NOT NULL,
	[ModifiedUtc] [datetime] NOT NULL,
	[DsrNumber] [char](6) NOT NULL,
 CONSTRAINT [PK_BranchSupport.DsrAliases] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Index [IX_UserId]    Script Date: 10/27/2016 1:05:25 PM ******/
CREATE NONCLUSTERED INDEX [IX_UserId] ON [BranchSupport].[DsrAliases]
(
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON

GO
ALTER TABLE [BranchSupport].[DsrAliases] ADD  DEFAULT (getutcdate()) FOR [CreatedUtc]
GO
ALTER TABLE [BranchSupport].[DsrAliases] ADD  DEFAULT (getutcdate()) FOR [ModifiedUtc]
GO
ALTER TABLE [BranchSupport].[DsrAliases] ADD  DEFAULT ('') FOR [DsrNumber]
GO
