declare @Settings as TABLE 
( 
    [Id]                 BIGINT         NOT NULL    PRIMARY KEY    IDENTITY (1, 1),
    [BranchName]         NVARCHAR (MAX) NULL,
    [BranchId]           NVARCHAR (MAX) NULL,
    [SupportPhoneNumber] NVARCHAR (MAX) NULL,
    [TollFreeNumber]     NVARCHAR (MAX) NULL,
    [Email]              NVARCHAR (MAX) NULL,
    [CreatedUtc]         DATETIME       DEFAULT (getutcdate()) NOT NULL,
    [ModifiedUtc]        DATETIME       DEFAULT (getutcdate()) NOT NULL
)

INSERT 
	INTO @Settings 
		([BranchName], [BranchId], [SupportPhoneNumber], [TollFreeNumber], [Email]) 
	VALUES 
		('Elba', 'FEL', '3348530384', '', 'Fel-dis-itadmin@benekeith.com')

MERGE INTO [BranchSupport].[BranchSupports] A
USING @Settings B ON (A.[BranchId] = B.[BranchId])
WHEN MATCHED THEN
    UPDATE SET A.[BranchName] = B.[BranchName], A.[SupportPhoneNumber] = B.[SupportPhoneNumber], A.[TollFreeNumber] = B.[TollFreeNumber], A.[Email] = B.[Email], A.[ModifiedUtc] = GetDate()
WHEN NOT MATCHED THEN
    INSERT ([BranchName], [BranchId], [SupportPhoneNumber], [TollFreeNumber], [Email]) 
	  VALUES(B.[BranchName],B.[BranchId],B.[SupportPhoneNumber],B.[TollFreeNumber],B.[Email]);