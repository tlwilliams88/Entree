CREATE INDEX idx_BranchCustomerNumber
	ON	[List].[CustomListHeaders] (
		BranchId,
		CustomerNumber
	)
GO