CREATE PROCEDURE [List].[AddOrUpdateRecommendedItemByCustomerNumberBranch] 
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

	MERGE INTO [BEK_Commerce_AppData].[List].[RecommendedItemsHeaders] A
	USING @Header B ON (A.[CustomerNumber] = B.[CustomerNumber] and A.[BranchId] = B.[BranchId])
	WHEN NOT MATCHED THEN
		INSERT ([CustomerNumber], [BranchId]) 
		VALUES (B.CustomerNumber, B.BranchId);
	  		
	declare @ParentRecommendedItemsHeaderId as bigint =
		(
		SELECT
			[Id]
		FROM [List].[RecommendedItemsHeaders] 
		WHERE	[CustomerNumber] = @CustomerNumber
				AND [BranchId] = @BranchId
		)
			
	declare @Detail as TABLE 
	( 
		[Id]                  BIGINT         IDENTITY (1, 1) NOT NULL,
		[ParentRecommendedItemsHeaderId] BIGINT         DEFAULT ((0)) NOT NULL,
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
			([ParentRecommendedItemsHeaderId], [ItemNumber], [Each], [CatalogId], [Active]) 
		VALUES 
			(@ParentRecommendedItemsHeaderId, @ItemNumber, @Each, @CatalogId, @Active)

	MERGE INTO [BEK_Commerce_AppData].[List].[RecommendedItemDetails] A
	USING @Detail B ON (A.[ParentRecommendedItemsHeaderId] = B.[ParentRecommendedItemsHeaderId] and A.[ItemNumber] = B.[ItemNumber] and A.[Each] = B.[Each] and A.[CatalogId] = B.[CatalogId])
	WHEN MATCHED THEN
	    UPDATE SET A.[Active] = B.[Active], A.[ModifiedUtc] = B.[ModifiedUtc]
	WHEN NOT MATCHED THEN
		INSERT
			([ParentRecommendedItemsHeaderId], [ItemNumber], [Each], [CatalogId], [Active]) 
		VALUES 
			(B.ParentRecommendedItemsHeaderId, B.ItemNumber, B.Each, B.CatalogId, B.Active);
		