CREATE INDEX idx_ParentFavoriteHeaderId_Active
	ON [List].[FavoritesDetails] (
		[ParentFavoritesHeaderId],
		[Active]
	)
GO
