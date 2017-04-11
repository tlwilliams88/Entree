/****** Object:  Table [Invoice].[Terms]    Script Date: 10/27/2016 1:05:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [Invoice].[Terms](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[BranchId] [nvarchar](3) NULL,
	[TermCode] [int] NOT NULL,
	[Description] [nvarchar](25) NULL,
	[Age1] [int] NOT NULL,
	[Age2] [int] NOT NULL,
	[Age3] [int] NOT NULL,
	[Age4] [int] NOT NULL,
	[CreatedUtc] [datetime] NOT NULL,
	[ModifiedUtc] [datetime] NOT NULL,
 CONSTRAINT [PK_Invoice.Terms] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
ALTER TABLE [Invoice].[Terms] ADD  DEFAULT (getutcdate()) FOR [CreatedUtc]
GO
ALTER TABLE [Invoice].[Terms] ADD  DEFAULT (getutcdate()) FOR [ModifiedUtc]
GO
