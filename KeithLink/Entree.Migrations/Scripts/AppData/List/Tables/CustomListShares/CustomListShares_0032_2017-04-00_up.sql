CREATE TABLE [List].[CustomListShares] (
    [Id]				BIGINT			   PRIMARY KEY IDENTITY(1,1) NOT NULL,
    [CustomerNumber]    [nvarchar](10)	   NULL,
    [BranchId]		    [nvarchar](10)	   NULL,
    [ParentCustomListHeaderId]	   	       bigint	   NOT NULL,
	[Active] BIT DEFAULT (1) NOT NULL,
    [CreatedUtc]        DATETIME           DEFAULT (getutcdate()) NOT NULL,
    [ModifiedUtc]       DATETIME           DEFAULT (getutcdate()) NOT NULL
);
GO
