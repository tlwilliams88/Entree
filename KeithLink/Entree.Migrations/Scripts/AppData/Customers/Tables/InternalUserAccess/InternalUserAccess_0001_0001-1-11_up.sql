/****** Object:  Table [Customers].[InternalUserAccess]    Script Date: 10/27/2016 1:05:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [Customers].[InternalUserAccess](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[BranchId] [char](3) NOT NULL,
	[CustomerNumber] [char](6) NULL,
	[UserId] [uniqueidentifier] NOT NULL,
	[CustomerId] [uniqueidentifier] NOT NULL,
	[RoleId] [varchar](70) NOT NULL,
	[EmailAddress] [varchar](200) NOT NULL,
	[CreatedUtc] [datetime] NOT NULL DEFAULT (getutcdate()),
	[ModifiedUtc] [datetime] NOT NULL DEFAULT (getutcdate()),
 CONSTRAINT [PK_Customers.InternalUserAccess] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Index [IdxByEmailAddress]    Script Date: 10/27/2016 1:05:25 PM ******/
CREATE NONCLUSTERED INDEX [IdxByEmailAddress] ON [Customers].[InternalUserAccess]
(
	[EmailAddress] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [IdxInternalUser]    Script Date: 10/27/2016 1:05:25 PM ******/
CREATE NONCLUSTERED INDEX [IdxInternalUser] ON [Customers].[InternalUserAccess]
(
	[BranchId] ASC,
	[CustomerNumber] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 80) ON [PRIMARY]
GO
SET ANSI_PADDING ON

GO
