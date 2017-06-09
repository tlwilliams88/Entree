CREATE TABLE [List].[MandatoryItemsHeaders] (
    [Id]				BIGINT PRIMARY KEY IDENTITY(1,1) NOT NULL,
	[CustomerNumber]    CHAR(6) NOT NULL,
    [BranchId]		    CHAR(3) NOT NULL,
    [Name]			    VARCHAR(40) NULL DEFAULT 'Mandatory',
    [CreatedUtc]        DATETIME DEFAULT (getutcdate()) NOT NULL,
    [ModifiedUtc]       DATETIME DEFAULT (getutcdate()) NOT NULL
);
GO