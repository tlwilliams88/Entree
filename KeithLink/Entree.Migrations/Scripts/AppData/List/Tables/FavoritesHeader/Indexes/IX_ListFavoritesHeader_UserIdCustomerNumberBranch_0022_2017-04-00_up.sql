CREATE UNIQUE INDEX [IX_ListFavoritesHeader_UserIdCustomerNumberBranch] ON [List].[FavoritesHeader]
(
	[UserId], [CustomerNumber], [BranchId]
)
GO