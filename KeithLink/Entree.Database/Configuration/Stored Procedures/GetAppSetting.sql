		CREATE PROCEDURE [Configuration].[GetAppSetting]
			@Key		varchar(50)
		AS
			SELECT 
 				[Key], 
				Value,
				Comment,
				[Disabled]
 			FROM 
 				[Configuration].[AppSettings]
 			WHERE
 				[Key] = @Key