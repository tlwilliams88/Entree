
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