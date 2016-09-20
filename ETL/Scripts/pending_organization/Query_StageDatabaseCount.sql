USE master;

DECLARE @KeithlinkDbCount int;
SET @KeithlinkDbCount = (
	SELECT
		COUNT(*)
	FROM
		Sys.Databases
	WHERE
		Name = 'KeithLinkAppData'
);
SELECT 
	@KeithlinkDbCount 'DbCount';
GO