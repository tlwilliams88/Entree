CREATE PROCEDURE [List].[GetContractItemsByHeaderId]
	@HeaderId INT
AS 
	SELECT
		ci.Id,
		ci.HeaderId,
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
		[List].[ContractDetails] ci
	WHERE
		ci.HeaderId = @HeaderId
