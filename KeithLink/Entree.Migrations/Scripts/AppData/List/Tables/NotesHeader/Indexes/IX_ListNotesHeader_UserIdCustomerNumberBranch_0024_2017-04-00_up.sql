CREATE UNIQUE INDEX [IX_ListNotesHeader_CustomerNumberBranch] ON [List].[NotesHeader]
(
	[CustomerNumber], [BranchId]
)
GO