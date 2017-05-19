declare @Settings as TABLE 
( 
    [Id]                 BIGINT         IDENTITY (1, 1) NOT NULL,
    [BranchName]         NVARCHAR (MAX) NULL,
    [BranchId]           NVARCHAR (MAX) NULL,
    [SupportPhoneNumber] NVARCHAR (MAX) NULL,
    [TollFreeNumber]     NVARCHAR (MAX) NULL,
    [Email]              NVARCHAR (MAX) NULL,
    [CreatedUtc]         DATETIME       DEFAULT (getutcdate()) NOT NULL,
    [ModifiedUtc]        DATETIME       DEFAULT (getutcdate()) NOT NULL,
    CONSTRAINT [PK_BranchSupport.BranchSupports] PRIMARY KEY CLUSTERED ([Id] ASC)
)

INSERT 
	INTO @Settings 
		([BranchName], [BranchId], [SupportPhoneNumber], [TollFreeNumber], [Email]) 
	VALUES 
		('Elba', 'FEL', '3348530384', '', 'Fel-dis-itadmin@benekeith.com')

MERGE INTO [BEK_Commerce_AppData].[BranchSupports].[BranchSupport] A
USING @AppSettings B ON (A.[BranchId] = B.[BranchId])
WHEN MATCHED THEN
    UPDATE SET A.[BranchName] = B.[BranchName], A.[SupportPhoneNumber] = B.[SupportPhoneNumber], A.[TollFreeNumber] = B.[TollFreeNumber], A.[Email] = B.[Email], A.[ModifiedUtc] = GetDate()
WHEN NOT MATCHED THEN
    INSERT ([BranchName], [BranchId], [SupportPhoneNumber], [TollFreeNumber], [Email]) 
	  VALUES(B.[BranchName],B.[BranchId],B.[SupportPhoneNumber],B.[TollFreeNumber],B.[Email]);