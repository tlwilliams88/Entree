INSERT INTO [List].[ContractHeaders]
(
	[ContractId],
	[BranchId],
	[CustomerNumber],
	[Name]
)
SELECT 
	LTRIM(RTRIM(SUBSTRING([DisplayName], 11, 10))), --Need to get the ContractId
	BranchId,
	CustomerId, 
	DisplayName
FROM
	[List].[Lists]
WHERE
	[Type] = 2