USE master;

DECLARE @SSISLogDbCount int;
SET @SSISLogDbCount = (
	SELECT
		COUNT(*)
	FROM
		Sys.Databases
	WHERE
		Name = 'KeithLinkAppData'
);
SELECT 
	@SSISLogDbCount 'DbCount';
GO