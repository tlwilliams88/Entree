SET ANSI_NULLS ON

SET QUOTED_IDENTIFIER ON

SET ANSI_PADDING ON

IF (NOT EXISTS (SELECT * FROM sys.databases 
				WHERE name = '${databaseName}'))
  BEGIN
    CREATE DATABASE [${databaseName}]
  END