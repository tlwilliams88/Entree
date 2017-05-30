CREATE PROCEDURE [List].[AddOrUpdateInventoryValuationItemByCustomerNumberBranch] 
	@CustomerNumber	NVARCHAR (10),
	@BranchId		NVARCHAR (10),
	@ListId			BIGINT,
	@ListName		VARCHAR (40),
	@ItemNumber		NVARCHAR (15),
	@Each           BIT,
	@Quantity       DECIMAL (18, 2),
	@CatalogId      NVARCHAR (24),
	@Active			BIT
AS
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	declare @Header as TABLE 
	( 
		[Id]             BIGINT           NOT NULL,
		[CustomerNumber] NVARCHAR (10)    NULL,
		[BranchId]       NVARCHAR (10)    NULL,
		[Name]           VARCHAR (40)     NULL,
		[CreatedUtc]     DATETIME         DEFAULT (getutcdate()) NOT NULL,
		[ModifiedUtc]    DATETIME         DEFAULT (getutcdate()) NOT NULL,
		PRIMARY KEY CLUSTERED ([Id] ASC)
	)

	INSERT 
		INTO @Header 
			([Id], [CustomerNumber], [BranchId], [Name]) 
		VALUES 
			(@ListId, @CustomerNumber, @BranchId, @ListName)

	MERGE INTO [BEK_Commerce_AppData].[List].[InventoryValuationListHeaders] A
	USING @Header B ON (A.[CustomerNumber] = B.[CustomerNumber] and A.[BranchId] = B.[BranchId] AND A.[Name] = @ListName)
	WHEN NOT MATCHED THEN
		INSERT ([CustomerNumber], [BranchId], [Name]) 
		VALUES (B.CustomerNumber, B.BranchId, B.Name);
	  		
	declare @ParentInventoryValuationListHeaderId as bigint =
		(
		SELECT
			[Id]
		FROM [List].[InventoryValuationListHeaders] 
		WHERE	[CustomerNumber] = @CustomerNumber
				AND [BranchId] = @BranchId
				AND [Name] = @ListName
		)
			
	declare @Detail as TABLE 
	( 
		[Id]                  BIGINT         IDENTITY (1, 1) NOT NULL,
		[ParentInventoryValuationListHeaderId] BIGINT         DEFAULT ((0)) NOT NULL,
		[ItemNumber]          NVARCHAR (15)  NOT NULL,
		[Each]                BIT            NULL,
		[CatalogId]           NVARCHAR (24)  NULL,
		[Quantity]            DECIMAL (18, 2) NOT NULL,
		[Active]              BIT            NULL,
		[CreatedUtc]          DATETIME       DEFAULT (getutcdate()) NOT NULL,
		[ModifiedUtc]         DATETIME       DEFAULT (getutcdate()) NOT NULL,
		PRIMARY KEY CLUSTERED ([Id] ASC)
	)

	INSERT 
		INTO @Detail 
			([ParentInventoryValuationListHeaderId], [ItemNumber], [Each], [CatalogId], [Quantity], [Active]) 
		VALUES 
			(@ParentInventoryValuationListHeaderId, @ItemNumber, @Each, @CatalogId, @Quantity, @Active)

	MERGE INTO [BEK_Commerce_AppData].[List].[InventoryValuationListDetails] A
	USING @Detail B ON (A.[ParentInventoryValuationListHeaderId] = B.[ParentInventoryValuationListHeaderId] and A.[ItemNumber] = B.[ItemNumber] and A.[Each] = B.[Each] and A.[CatalogId] = B.[CatalogId])
	WHEN MATCHED THEN
	    UPDATE SET A.[Quantity] = B.[Quantity], A.[Active] = B.[Active], A.[CatalogId] = B.[CatalogId], A.[ModifiedUtc] = B.[ModifiedUtc]
	WHEN NOT MATCHED THEN
		INSERT
			([ParentInventoryValuationListHeaderId], [ItemNumber], [Each], [CatalogId], [Quantity], [Active]) 
		VALUES 
			(B.ParentInventoryValuationListHeaderId, B.ItemNumber, B.Each, B.CatalogId, B.Quantity, B.Active);
		