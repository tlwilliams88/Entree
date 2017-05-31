CREATE TABLE [List].[FavoritesHeaders] (
    [Id]				BIGINT			   PRIMARY KEY IDENTITY(1,1) NOT NULL,
	[UserId]            [uniqueidentifier] NULL,
    [CustomerNumber]    [nvarchar](10)	   NULL,
    [BranchId]		    [nvarchar](10)	   NULL,
    [Name]			    [nvarchar](40)	   NULL DEFAULT 'Favorites',
    [CreatedUtc]        DATETIME           DEFAULT (getutcdate()) NOT NULL,
    [ModifiedUtc]       DATETIME           DEFAULT (getutcdate()) NOT NULL
);
GO
