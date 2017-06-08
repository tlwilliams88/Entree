CREATE TABLE [List].[FavoritesHeaders] (
    [Id]				BIGINT			    NOT NULL     PRIMARY KEY IDENTITY(1,1),
	[UserId]            UNIQUEIDENTIFIER    NULL,
    [BranchId]          CHAR(3)             NOT NULL,
    [CustomerNumber]    CHAR(6)             NOT NULL,
    [Name]			    CHAR(9) 	        NULL         DEFAULT 'Favorites',
    [CreatedUtc]        DATETIME            NOT NULL     DEFAULT (getutcdate()),
    [ModifiedUtc]       DATETIME            NOT NULL     DEFAULT (getutcdate())
)
GO
