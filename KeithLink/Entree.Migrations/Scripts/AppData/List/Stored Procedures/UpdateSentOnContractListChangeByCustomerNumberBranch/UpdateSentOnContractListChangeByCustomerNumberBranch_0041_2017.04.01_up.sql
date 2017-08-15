-- Stored Procedure for reading the next set of contract list changes
CREATE PROCEDURE [List].[UpdateSentOnContractListChangeByCustomerNumberBranch]
	@CustomerNumber   CHAR(6),
	@BranchId   CHAR(3),
	@Sent BIT 
AS
BEGIN
	UPDATE 
		[List].[ListItemsDelta]
		SET [Sent] = @Sent
		WHERE [CustomerNumber] = @CustomerNumber
            AND [BranchId] = @BranchId
END
GO