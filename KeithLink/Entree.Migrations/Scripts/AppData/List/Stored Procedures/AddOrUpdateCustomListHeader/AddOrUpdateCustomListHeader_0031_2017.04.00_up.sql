CREATE PROCEDURE [List].[AddOrUpdateCustomListHeader] 
	@UserId [uniqueidentifier],
	@BranchId CHAR(3),
	@CustomerNumber CHAR(6),
	@Name VARCHAR(30),
	@Active         BIT
AS
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
			
	declare @Header as TABLE 
	( 
		[Id]             BIGINT           IDENTITY (1, 1) NOT NULL,
		[UserId]         UNIQUEIDENTIFIER NULL,
		[CustomerNumber] NVARCHAR (10)    NULL,
		[BranchId]       NVARCHAR (10)    NULL,
		[Name]           NVARCHAR (MAX)   NULL,
		[Active]		 BIT DEFAULT (1) NOT NULL,
		[CreatedUtc]     DATETIME         DEFAULT (getutcdate()) NOT NULL,
		[ModifiedUtc]    DATETIME         DEFAULT (getutcdate()) NOT NULL,
		PRIMARY KEY CLUSTERED ([Id] ASC)
	)

	INSERT 
		INTO @Header 
			([UserId], [CustomerNumber], [BranchId], [Name], [Active]) 
		VALUES 
			(@UserId, @CustomerNumber, @BranchId, @Name, @Active)

	MERGE INTO [BEK_Commerce_AppData].[List].[CustomListHeaders] A
	USING @Header B ON (A.[UserId] = B.[UserId] and A.[CustomerNumber] = B.[CustomerNumber] and A.[BranchId] = B.[BranchId])
	WHEN MATCHED THEN
	    UPDATE SET A.[Name] = B.[Name], A.[Active] = B.[Active], A.[ModifiedUtc] = B.[ModifiedUtc]
	WHEN NOT MATCHED THEN
		INSERT ([UserId], [CustomerNumber], [BranchId], [Name]) 
		VALUES (B.UserId, B.CustomerNumber, B.BranchId, B.Name);
		