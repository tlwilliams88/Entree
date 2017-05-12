/****** Object:  StoredProcedure [Configuration].[CheckAppSettingsForChange]    Script Date: 10/27/2016 1:05:24 PM ******/
SET ANSI_NULLS OFF
GO
SET QUOTED_IDENTIFIER OFF
GO
 CREATE PROCEDURE [Configuration].[CheckAppSettingsForChange]
 AS
 BEGIN
 SET NOCOUNT ON;
   SELECT 
     Value
   FROM 
     [Configuration].[AppSettings]
   WHERE
       [Key] = 'DBChangeValue'
 END

GO
