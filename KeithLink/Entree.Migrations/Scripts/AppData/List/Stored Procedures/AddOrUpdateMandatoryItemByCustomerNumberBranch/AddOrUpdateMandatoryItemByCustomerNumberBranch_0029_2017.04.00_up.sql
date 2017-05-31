CREATE PROCEDURE [List].[AddOrUpdateMandatoryItemByCustomerNumberBranch] 
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

	MERGE INTO [BEK_Commerce_AppData].[List].[MandatoryItemsHeaders] A
	USING @Header B ON (A.[CustomerNumber] = B.[CustomerNumber] and A.[BranchId] = B.[BranchId])
	WHEN NOT MATCHED THEN
		INSERT ([CustomerNumber], [BranchId]) 
		VALUES (B.CustomerNumber, B.BranchId);
	  		
	declare @ParentMandatoryItemsHeaderId as bigint =
		(
		SELECT
			[Id]
		FROM [List].[MandatoryItemsHeaders] 
		WHERE	[CustomerNumber] = @CustomerNumber
				AND [BranchId] = @BranchId
		)
			
	declare @Detail as TABLE 
	( 
		[Id]                  BIGINT         IDENTITY (1, 1) NOT NULL,
		[ParentMandatoryItemsHeaderId] BIGINT         DEFAULT ((0)) NOT NULL,
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
			([ParentMandatoryItemsHeaderId], [ItemNumber], [Each], [CatalogId], [Active]) 
		VALUES 
			(@ParentMandatoryItemsHeaderId, @ItemNumber, @Each, @CatalogId, @Active)

	MERGE INTO [BEK_Commerce_AppData].[List].[MandatoryItemsDetails] A
	USING @Detail B ON (A.[ParentMandatoryItemsHeaderId] = B.[ParentMandatoryItemsHeaderId] and A.[ItemNumber] = B.[ItemNumber] and A.[Each] = B.[Each] and A.[CatalogId] = B.[CatalogId])
	WHEN MATCHED THEN
	    UPDATE SET A.[Active] = B.[Active], A.[ModifiedUtc] = B.[ModifiedUtc]
	WHEN NOT MATCHED THEN
		INSERT
			([ParentMandatoryItemsHeaderId], [ItemNumber], [Each], [CatalogId], [Active]) 
		VALUES 
			(B.ParentMandatoryItemsHeaderId, B.ItemNumber, B.Each, B.CatalogId, B.Active);
		