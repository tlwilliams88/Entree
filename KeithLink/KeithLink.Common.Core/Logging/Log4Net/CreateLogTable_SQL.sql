USE [${databaseName}]

SET ANSI_NULLS ON

SET QUOTED_IDENTIFIER ON

SET ANSI_PADDING ON

IF (NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES 
					WHERE TABLE_SCHEMA = 'dbo' 
					AND  TABLE_NAME = 'Log'))
BEGIN

	CREATE TABLE [dbo].[Log](
		[Id] [int] IDENTITY(1,1) NOT NULL,
		[Date] [datetime] NOT NULL,
		[Thread] [varchar](255) NOT NULL,
		[Host] [varchar] (255) NOT NULL,
		[User] [varchar] (255) NOT NULL,
		[Application] [varchar] (255) NOT NULL,
		[Level] [varchar](50) NOT NULL,
		[Logger] [varchar](255) NOT NULL,
		[Message] [varchar](MAX) NOT NULL,
		[Exception] [varchar](MAX) NULL
	) ON [PRIMARY]

	SET ANSI_PADDING OFF

	CREATE UNIQUE CLUSTERED INDEX PK_Log ON [Log] 
		([Id]) 

	CREATE INDEX IX_Log_Date
		ON [dbo].[Log] ([Date]);

END

