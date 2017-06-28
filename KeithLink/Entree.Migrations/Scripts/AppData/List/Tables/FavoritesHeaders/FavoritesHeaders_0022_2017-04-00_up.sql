CREATE TABLE [List].[FavoritesHeaders] (
    [Id]				BIGINT			    NOT NULL     PRIMARY KEY IDENTITY(1,1),
	[UserId]            UNIQUEIDENTIFIER    NULL,
    [BranchId]          CHAR(3)             NOT NULL,
    [CustomerNumber]    CHAR(6)             NOT NULL,
    [CreatedUtc]        DATETIME            NOT NULL,
    [ModifiedUtc]       DATETIME            NOT NULL     
)
GO
