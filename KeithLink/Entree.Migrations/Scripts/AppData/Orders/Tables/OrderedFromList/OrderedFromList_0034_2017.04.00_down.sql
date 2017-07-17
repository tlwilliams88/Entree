UPDATE [Orders].[OrderedFromList] SET [ListType] = NULL
GO
ALTER TABLE [Orders].[OrderedFromList]
	DROP COLUMN [ListType]
GO

