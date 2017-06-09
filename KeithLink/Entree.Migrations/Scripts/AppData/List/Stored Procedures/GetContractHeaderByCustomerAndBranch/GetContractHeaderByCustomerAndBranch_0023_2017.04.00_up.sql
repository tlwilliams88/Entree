CREATE PROCEDURE [List].[GetContractHeaderByCustomerAndBranch]
	@BranchId		CHAR(3),
	@CustomerNumber CHAR(6)
AS
	SELECT 
		ch.Id,
		ch.ContractId,
		ch.BranchId,
		ch.CustomerNumber,
		ch.CreatedUtc,
		ch.ModifiedUtc
	FROM
		[List].[ContractHeaders] ch
	WHERE
		ch.BranchId = @BranchId
	AND
		ch.CustomerNumber = @CustomerNumber
GO
