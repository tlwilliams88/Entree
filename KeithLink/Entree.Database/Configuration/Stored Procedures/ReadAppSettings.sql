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