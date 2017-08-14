/****** Object:  StoredProcedure [Configuration].[GetAppSetting]    Script Date: 10/27/2016 1:05:24 PM ******/
SET ANSI_NULLS OFF
GO
SET QUOTED_IDENTIFIER OFF
GO
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

GO
