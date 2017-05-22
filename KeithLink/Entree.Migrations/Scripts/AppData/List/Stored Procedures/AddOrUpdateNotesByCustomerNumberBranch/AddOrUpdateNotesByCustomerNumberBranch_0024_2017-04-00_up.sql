CREATE PROCEDURE [List].[AddOrUpdateNotesByCustomerNumberBranch] 
	@CustomerNumber	NVARCHAR (10),
	@BranchId		NVARCHAR (10),
	@ItemNumber		NVARCHAR (15),
	@Each           BIT,
	@CatalogId      NVARCHAR (24),
	@Note			NVARCHAR (max),
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
		[Name]           NVARCHAR (MAX)   NULL,
		[CreatedUtc]     DATETIME         DEFAULT (getutcdate()) NOT NULL,
		[ModifiedUtc]    DATETIME         DEFAULT (getutcdate()) NOT NULL,
		PRIMARY KEY CLUSTERED ([Id] ASC)
	)

	INSERT 
		INTO @Header 
			([CustomerNumber], [BranchId], [Name]) 
		VALUES 
			(@CustomerNumber, @BranchId, N'Notes')

	MERGE INTO [BEK_Commerce_AppData].[List].[NotesHeader] A
	USING @Header B ON (A.[CustomerNumber] = B.[CustomerNumber] and A.[BranchId] = B.[BranchId])
	WHEN NOT MATCHED THEN
		INSERT ([CustomerNumber], [BranchId], [Name]) 
		VALUES (B.CustomerNumber, B.BranchId, B.Name);
	  		
	declare @ParentNotesHeaderId as bigint =
		(
		SELECT
			[Id]
		FROM [List].[NotesHeader] 
		WHERE	[CustomerNumber] = @CustomerNumber
				AND [BranchId] = @BranchId
		)
			
	declare @Detail as TABLE 
	( 
		[Id]                  BIGINT         IDENTITY (1, 1) NOT NULL,
		[ParentNotesHeaderId] BIGINT         DEFAULT ((0)) NOT NULL,
		[ItemNumber]          NVARCHAR (15)  NOT NULL,
		[Each]                BIT            NULL,
		[CatalogId]           NVARCHAR (24)  NULL,
		[Note]                NVARCHAR (MAX) NULL,
		[Active]                BIT            NULL,
		[CreatedUtc]          DATETIME       DEFAULT (getutcdate()) NOT NULL,
		[ModifiedUtc]         DATETIME       DEFAULT (getutcdate()) NOT NULL,
		PRIMARY KEY CLUSTERED ([Id] ASC)
	)

	INSERT 
		INTO @Detail 
			([ParentNotesHeaderId], [ItemNumber], [Each], [CatalogId], [Note], [Active]) 
		VALUES 
			(@ParentNotesHeaderId, @ItemNumber, @Each, @CatalogId, @Note, @Active)

	MERGE INTO [BEK_Commerce_AppData].[List].[NotesDetail] A
	USING @Detail B ON (A.[ParentNotesHeaderId] = B.[ParentNotesHeaderId] and A.[ItemNumber] = B.[ItemNumber] and A.[Each] = B.[Each] and A.[CatalogId] = B.[CatalogId])
	WHEN MATCHED THEN
	    UPDATE SET A.[Note] = B.[Note],A.[Active] = B.[Active], A.[ModifiedUtc] = B.[ModifiedUtc]
	WHEN NOT MATCHED THEN
		INSERT
			([ParentNotesHeaderId], [ItemNumber], [Each], [CatalogId], [Note], [Active]) 
		VALUES 
			(B.ParentNotesHeaderId, B.ItemNumber, B.Each, B.CatalogId, B.Note, B.Active);
		