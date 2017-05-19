CREATE PROCEDURE [List].[GetContractHeadersByCustomerAndBranch]
	@CustomerNumber CHAR(6),
	@Branch CHAR(3)
AS
	SELECT 
		ch.Id,
		ch.ContractId,
		ch.Branch,
		ch.CustomerNumber,
		ch.Name,
		ch.CreatedAt,
		ch.ModifiedOn
	FROM
		[List].[ContractHeaders] ch
	WHERE
		ch.CustomerNumber = @CustomerNumber AND
		ch.Branch = @Branch

GO
