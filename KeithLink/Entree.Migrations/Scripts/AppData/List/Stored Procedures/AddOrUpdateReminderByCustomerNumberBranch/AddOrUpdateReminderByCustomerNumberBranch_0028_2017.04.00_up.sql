CREATE PROCEDURE [List].[AddOrUpdateReminderByCustomerNumberBranch] 
	@CustomerNumber	NVARCHAR (10),
	@BranchId		NVARCHAR (10),
	@ItemNumber		NVARCHAR (15),
	@Each           BIT,
	@CatalogId      NVARCHAR (24),
	@Active			BIT
AS
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	declare @Header as TABLE 
	( 
		[Id]             BIGINT           IDENTITY (1, 1) NOT NULL,
		[CustomerNumber] NVARCHAR (10)    NULL,
		[BranchId]       NVARCHAR (10)    NULL,
		[CreatedUtc]     DATETIME         DEFAULT (getutcdate()) NOT NULL,
		[ModifiedUtc]    DATETIME         DEFAULT (getutcdate()) NOT NULL,
		PRIMARY KEY CLUSTERED ([Id] ASC)
	)

	INSERT 
		INTO @Header 
			([CustomerNumber], [BranchId]) 
		VALUES 
			(@CustomerNumber, @BranchId)

	MERGE INTO [BEK_Commerce_AppData].[List].[RemindersHeaders] A
	USING @Header B ON (A.[CustomerNumber] = B.[CustomerNumber] and A.[BranchId] = B.[BranchId])
	WHEN NOT MATCHED THEN
		INSERT ([CustomerNumber], [BranchId]) 
		VALUES (B.CustomerNumber, B.BranchId);
	  		
	declare @ParentRemindersHeaderId as bigint =
		(
		SELECT
			[Id]
		FROM [List].[RemindersHeaders] 
		WHERE	[CustomerNumber] = @CustomerNumber
				AND [BranchId] = @BranchId
		)
			
	declare @Detail as TABLE 
	( 
		[Id]                  BIGINT         IDENTITY (1, 1) NOT NULL,
		[ParentRemindersHeaderId] BIGINT         DEFAULT ((0)) NOT NULL,
		[ItemNumber]          NVARCHAR (15)  NOT NULL,
		[Each]                BIT            NULL,
		[CatalogId]           NVARCHAR (24)  NULL,
		[Active]                BIT            NULL,
		[CreatedUtc]          DATETIME       DEFAULT (getutcdate()) NOT NULL,
		[ModifiedUtc]         DATETIME       DEFAULT (getutcdate()) NOT NULL,
		PRIMARY KEY CLUSTERED ([Id] ASC)
	)

	INSERT 
		INTO @Detail 
			([ParentRemindersHeaderId], [ItemNumber], [Each], [CatalogId], [Active]) 
		VALUES 
			(@ParentRemindersHeaderId, @ItemNumber, @Each, @CatalogId, @Active)

	MERGE INTO [BEK_Commerce_AppData].[List].[ReminderDetails] A
	USING @Detail B ON (A.[ParentRemindersHeaderId] = B.[ParentRemindersHeaderId] and A.[ItemNumber] = B.[ItemNumber] and A.[Each] = B.[Each] and A.[CatalogId] = B.[CatalogId])
	WHEN MATCHED THEN
	    UPDATE SET A.[Active] = B.[Active], A.[ModifiedUtc] = B.[ModifiedUtc]
	WHEN NOT MATCHED THEN
		INSERT
			([ParentRemindersHeaderId], [ItemNumber], [Each], [CatalogId], [Active]) 
		VALUES 
			(B.ParentRemindersHeaderId, B.ItemNumber, B.Each, B.CatalogId, B.Active);
		