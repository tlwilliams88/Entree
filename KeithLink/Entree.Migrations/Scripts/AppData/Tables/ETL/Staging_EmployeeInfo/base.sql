/****** Object:  Table [ETL].[Staging_EmployeeInfo]    Script Date: 10/27/2016 1:05:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [ETL].[Staging_EmployeeInfo](
	[EMPLID] [varchar](11) NOT NULL,
	[EMAIL_ADDR] [varchar](70) NULL,
	[PHONE] [varchar](24) NULL,
	[EMPLOYEE_PHOTO] [varbinary](max) NULL,
	[LAST_NAME] [varchar](30) NULL,
	[FIRST_NAME] [varchar](30) NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
