CREATE PROCEDURE [List].[AddOrUpdateRecentlyViewedByUserIdCustomerNumberBranch] 
	@UserId			[uniqueidentifier], 
	@CustomerNumber	NVARCHAR (10),
	@BranchId		NVARCHAR (10),
	@ItemNumber		NVARCHAR (15),
	@Each           BIT,
	@CatalogId      NVARCHAR (24),
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
		[CreatedUtc]     DATETIME         DEFAULT (getutcdate()) NOT NULL,
		[ModifiedUtc]    DATETIME         DEFAULT (getutcdate()) NOT NULL,
		PRIMARY KEY CLUSTERED ([Id] ASC)
	)

	INSERT 
		INTO @Header 
			([UserId], [CustomerNumber], [BranchId], [Name]) 
		VALUES 
			(@UserId, @CustomerNumber, @BranchId, N'Favorites')

	MERGE INTO [BEK_Commerce_AppData].[List].[RecentlyViewedHeader] A
	USING @Header B ON (A.[UserId] = B.[UserId] and A.[CustomerNumber] = B.[CustomerNumber] and A.[BranchId] = B.[BranchId])
	WHEN NOT MATCHED THEN
		INSERT ([UserId], [CustomerNumber], [BranchId], [Name]) 
		VALUES (B.UserId, B.CustomerNumber, B.BranchId, B.Name);
	  		
	declare @ParentRecentlyViewedHeaderId as bigint =
		(
		SELECT
			[Id]
		FROM [List].[RecentlyViewedHeader] 
		WHERE	[UserId] = @UserId
				AND [CustomerNumber] = @CustomerNumber
				AND [BranchId] = @BranchId
		)
			
	declare @Detail as TABLE 
	( 
		[Id]                      BIGINT        IDENTITY (1, 1) NOT NULL,
		[ParentRecentlyViewedHeaderId] BIGINT        DEFAULT ((0)) NOT NULL,
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
			([ParentRecentlyViewedHeaderId], [ItemNumber], [Each], [CatalogId], Active) 
		VALUES 
			(@ParentRecentlyViewedHeaderId, @ItemNumber, @Each, @CatalogId, @Active)

	MERGE INTO [BEK_Commerce_AppData].[List].[RecentlyViewedDetail] A
	USING @Detail B ON (A.[ParentRecentlyViewedHeaderId] = B.[ParentRecentlyViewedHeaderId] and A.[ItemNumber] = B.[ItemNumber] and A.[Each] = B.[Each] and A.[CatalogId] = B.[CatalogId])
	WHEN NOT MATCHED THEN
		INSERT
			([ParentRecentlyViewedHeaderId], [ItemNumber], [Each], [CatalogId], [Active]) 
		VALUES 
			(B.ParentRecentlyViewedHeaderId, B.ItemNumber, B.Each, B.CatalogId, B.Active);