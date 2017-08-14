CREATE TABLE [List].[HistoryHeaders] (
    [Id]				BIGINT			PRIMARY KEY IDENTITY(1,1) NOT NULL,
    [BranchId]		    CHAR(3)	        NOT NULL,
    [CustomerNumber]    CHAR(6)	        NOT NULL,
    [CreatedUtc]        DATETIME,
    [ModifiedUtc]       DATETIME        
)
GO
