CREATE TABLE [List].[CustomListHeaders]
(
	[Id] int PRIMARY KEY IDENTITY(1,1),
	[UserId] [uniqueidentifier] NULL,
	[BranchId] VARCHAR(5) NOT NULL,
	[CustomerNumber] VARCHAR(10) NOT NULL,
	[Name] VARCHAR(200) NULL DEFAULT 'NoName',
	[Active] BIT DEFAULT (1) NOT NULL,
	[CreatedUtc] DATETIME NOT NULL DEFAULT GETUTCDATE(),
	[ModifiedUtc] DATETIME NOT NULL DEFAULT GETUTCDATE()
);

