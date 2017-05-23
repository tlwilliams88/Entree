CREATE UNIQUE INDEX [IX_ListNotesHeader_CustomerNumberBranch] ON [List].[NotesHeaders]
(
	[CustomerNumber], [BranchId]
)
GO