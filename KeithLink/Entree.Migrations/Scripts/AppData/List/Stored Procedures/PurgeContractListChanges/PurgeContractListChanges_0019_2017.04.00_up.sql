USE BEK_Commerce_AppData
GO
-- Stored Procedure for reading the next set of contract list changes
CREATE PROCEDURE [List].[PurgeContractListChanges]
			@PurgeDays		INT
AS
BEGIN
	DELETE TOP (100000) [List].[ListItemsDelta]
	WHERE
		CreatedUtc < DATEADD(day, @PurgeDays, GETDATE())
END
GO