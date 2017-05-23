-- =============================================
-- Author:			Matt Joiner
-- Create date:		05/22/2017
-- Description:	    Migrates the contract header and detail data post creation scripts to the new tables	
-- =============================================
	   
-- Migrate Header
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

-- Migrate Detail
INSERT INTO [List].[ContractDetails]
(
	[ParentContractHeaderId],
	[LineNumber],
	[ItemNumber],
	[FromDate],
	[ToDate],
	[Each],
	[Category],
	[CatalogId],
	[CreatedUtc],
	[ModifiedUtc]
)
SELECT 
	h.Id,
	i.Position,
	i.ItemNumber,
	i.FromDate,
	i.ToDate,
	i.Each,
	i.Category,
	i.CatalogId,
	GETUTCDATE(),
	GETUTCDATE()
FROM [List].[ListItems] i
LEFT JOIN [List].[Lists] h ON h.Id = i.ParentList_id
WHERE i.FromDate IS NOT NULL
AND i.ToDate IS NOT NULL