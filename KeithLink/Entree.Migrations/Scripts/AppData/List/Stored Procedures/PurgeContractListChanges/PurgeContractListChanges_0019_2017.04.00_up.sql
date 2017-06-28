CREATE PROCEDURE [List].[PurgeContractListChanges]
			@PurgeDays		INT
AS
BEGIN
	DELETE TOP (100000) [List].[ListItemsDelta]
	WHERE
		CreatedUtc < DATEADD(day, @PurgeDays, GETDATE())
END
GO