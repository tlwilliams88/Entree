CREATE TABLE [List].[CustomListShares] (
    [Id]				        BIGINT	    NOT NULL    PRIMARY KEY IDENTITY(1,1),
    [BranchId]		            CHAR(3)	    NOT NULL,
    [CustomerNumber]            CHAR(6)	    NOT NULL,
    [HeaderId]                  BIGINT      NOT NULL,
	[Active]                    BIT         NOT NULL,
    [CreatedUtc]                DATETIME    NOT NULL,
    [ModifiedUtc]               DATETIME    NOT NULL    
)
