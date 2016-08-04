CREATE FUNCTION ETL.ConcatBranchesForItem (
	@vendorId char(6),
	@itemId char(6)
)

RETURNS VARCHAR(50)
WITH SCHEMABINDING
AS
BEGIN

	DECLARE @ListOfBranches VARCHAR(50)

	SELECT @ListOfBranches = COALESCE(@ListOfBranches + ',', '') + BranchId
	FROM ETL.Staging_ItemData
	WHERE Vendor1 = @vendorId
	AND ItemId = @itemId

	RETURN (@ListOfBranches)

END