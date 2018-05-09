CREATE NONCLUSTERED INDEX [IX_GrowthAndRecoveries_BranchCustomerNumber] ON [Customers].[GrowthAndRecoveries]
(
	[BranchId] ASC,
	[CustomerNumber] ASC
)