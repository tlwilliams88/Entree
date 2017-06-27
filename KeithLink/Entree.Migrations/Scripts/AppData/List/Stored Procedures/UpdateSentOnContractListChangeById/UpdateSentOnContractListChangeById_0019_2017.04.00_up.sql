USE BEK_Commerce_AppData
GO
-- Stored Procedure for reading the next set of contract list changes
CREATE PROCEDURE [List].[UpdateSentOnContractListChangeById]
	@Id   BIGINT,
	@Sent BIT 
AS
BEGIN
	UPDATE 
		[List].[ListItemsDelta]
		SET [Sent] = @Sent
		WHERE [ParentList_Id] = @Id
END
GO