/****** Object:  StoredProcedure [Configuration].[ReadAppSettings]    Script Date: 10/27/2016 1:05:24 PM ******/
SET ANSI_NULLS OFF
GO
SET QUOTED_IDENTIFIER OFF
GO
 CREATE PROCEDURE [Configuration].[ReadAppSettings]
 AS
 BEGIN
 SET NOCOUNT ON;
 	SELECT 
 		[Key], 
		Value,
		Comment,
		[Disabled]
 	FROM 
 		[Configuration].[AppSettings]
 	WHERE
 	    [Disabled] = 0
 END

GO
