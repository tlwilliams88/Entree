CREATE PROCEDURE [List].[GetContractHeaderByCustomerAndBranch]
	@CustomerNumber CHAR(6),
	@BranchId CHAR(3)
AS
	SELECT 
		ch.Id,
		ch.ContractId,
		ch.BranchId,
		ch.CustomerNumber,
		ch.Name,
		ch.CreatedAt,
		ch.ModifiedUtc
	FROM
		[List].[ContractHeaders] ch
	WHERE
		ch.CustomerNumber = @CustomerNumber AND
		ch.BranchId = @BranchId

GO
