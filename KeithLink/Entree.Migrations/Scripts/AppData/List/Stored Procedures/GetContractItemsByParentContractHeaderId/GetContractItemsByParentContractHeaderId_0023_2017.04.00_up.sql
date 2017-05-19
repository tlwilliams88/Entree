CREATE PROCEDURE [List].[GetContractItemsByParentContractHeaderId]
	@ParentContractHeaderId INT
AS 
	SELECT
		ci.Id,
		ci.ParentContractHeaderId,
		ci.LineNumber,
		ci.ItemNumber,
		ci.FromDate,
		ci.ToDate,
		ci.Each,
		ci.Category,
		ci.CatalogId,
		ci.CreatedUtc,
		ci.ModifiedUtc
	FROM
		[List].[ContractItems] ci
	WHERE
		ci.ParentContractHeaderId = @ParentContractHeaderId