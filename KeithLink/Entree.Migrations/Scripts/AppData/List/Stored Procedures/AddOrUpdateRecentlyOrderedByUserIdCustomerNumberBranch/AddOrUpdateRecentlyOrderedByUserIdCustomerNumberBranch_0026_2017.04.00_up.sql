CREATE PROCEDURE [List].[AddOrUpdateRecentlyOrderedByUserIdCustomerNumberBranch] 
	@UserId			[uniqueidentifier], 
	@CustomerNumber	NVARCHAR (10),
	@BranchId		NVARCHAR (10),
	@ItemNumber		NVARCHAR (15),
	@Each           BIT,
	@CatalogId      NVARCHAR (24),
	@Active         BIT,
	@NumberToKeep	INT
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
		[CreatedUtc]     DATETIME         DEFAULT (getutcdate()) NOT NULL,
		[ModifiedUtc]    DATETIME         DEFAULT (getutcdate()) NOT NULL,
		PRIMARY KEY CLUSTERED ([Id] ASC)
	)

	INSERT 
		INTO @Header 
			([UserId], [CustomerNumber], [BranchId], [Name]) 
		VALUES 
			(@UserId, @CustomerNumber, @BranchId, N'Favorites')

	MERGE INTO [BEK_Commerce_AppData].[List].[RecentlyOrderedHeaders] A
	USING @Header B ON (A.[UserId] = B.[UserId] and A.[CustomerNumber] = B.[CustomerNumber] and A.[BranchId] = B.[BranchId])
	WHEN NOT MATCHED THEN
		INSERT ([UserId], [CustomerNumber], [BranchId], [Name]) 
		VALUES (B.UserId, B.CustomerNumber, B.BranchId, B.Name);
	  		
	declare @ParentRecentlyOrderedHeaderId as bigint =
		(
		SELECT
			[Id]
		FROM [List].[RecentlyOrderedHeaders] 
		WHERE	[UserId] = @UserId
				AND [CustomerNumber] = @CustomerNumber
				AND [BranchId] = @BranchId
		)
			
	declare @Detail as TABLE 
	( 
		[Id]                      BIGINT        IDENTITY (1, 1) NOT NULL,
		[ParentRecentlyOrderedHeaderId] BIGINT        DEFAULT ((0)) NOT NULL,
		[ItemNumber]              NVARCHAR (15) NOT NULL,
		[Each]                    BIT           NULL,
		[CatalogId]               NVARCHAR (24) NULL,
		[Active]                    BIT           NULL,
		[CreatedUtc]              DATETIME      DEFAULT (getutcdate()) NOT NULL,
		[ModifiedUtc]             DATETIME      DEFAULT (getutcdate()) NOT NULL,
		PRIMARY KEY CLUSTERED ([Id] ASC)
	)

	INSERT 
		INTO @Detail 
			([ParentRecentlyOrderedHeaderId], [ItemNumber], [Each], [CatalogId], Active) 
		VALUES 
			(@ParentRecentlyOrderedHeaderId, @ItemNumber, @Each, @CatalogId, @Active)

	MERGE INTO [BEK_Commerce_AppData].[List].[RecentlyOrderedDetails] A
	USING @Detail B ON (A.[ParentRecentlyOrderedHeaderId] = B.[ParentRecentlyOrderedHeaderId] and A.[ItemNumber] = B.[ItemNumber] and A.[Each] = B.[Each] and A.[CatalogId] = B.[CatalogId])
	WHEN NOT MATCHED THEN
		INSERT
			([ParentRecentlyOrderedHeaderId], [ItemNumber], [Each], [CatalogId], [Active]) 
		VALUES 
			(B.ParentRecentlyOrderedHeaderId, B.ItemNumber, B.Each, B.CatalogId, B.Active);

	DECLARE @Count AS INT = (
	SELECT count([Id]) FROM [BEK_Commerce_AppData].[List].[RecentlyOrderedDetails] WHERE [ParentRecentlyOrderedHeaderId] = @ParentRecentlyOrderedHeaderId
	)

	if(@Count > @NumberToKeep)
		DELETE FROM [BEK_Commerce_AppData].[List].[RecentlyOrderedDetails]
			WHERE [Id] IN
			(
				SELECT TOP (@Count - @NumberToKeep) [Id]
				FROM [BEK_Commerce_AppData].[List].[RecentlyOrderedDetails]
				WHERE [ParentRecentlyOrderedHeaderId] = @ParentRecentlyOrderedHeaderId
				ORDER BY [ModifiedUtc] ASC
			)