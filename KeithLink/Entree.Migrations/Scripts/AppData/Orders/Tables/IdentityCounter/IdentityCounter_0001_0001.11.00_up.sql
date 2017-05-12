/****** Object:  Table [Orders].[IdentityCounter]    Script Date: 10/27/2016 1:05:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [Orders].[IdentityCounter](
	[CounterName] [nvarchar](50) NULL,
	[StartId] [int] NULL,
	[EndId] [int] NULL,
	[CurrentId] [int] NULL
) ON [PRIMARY]

GO
