/****** Object:  StoredProcedure [ETL].[PurgeInternalUserAccess]    Script Date: 10/27/2016 1:05:24 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [ETL].[PurgeInternalUserAccess]
AS
TRUNCATE TABLE [Customers].[InternalUserAccess]
RETURN 0


GO
