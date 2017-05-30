CREATE PROCEDURE [List].[AddOrUpdateCustomListItemById] 
	@ParentCustomListHeaderId			BIGINT,
	@ItemNumber		NVARCHAR (15),
	@Each           BIT,
	@Par       DECIMAL (18, 2),
	@CatalogId      NVARCHAR (24),
	@Active			BIT
AS
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
			
	declare @Detail as TABLE 
	( 
		[Id]                  BIGINT         IDENTITY (1, 1) NOT NULL,
		[ParentCustomListHeaderId] BIGINT         DEFAULT ((0)) NOT NULL,
		[ItemNumber]          NVARCHAR (15)  NOT NULL,
		[Each]                BIT            NULL,
		[CatalogId]           NVARCHAR (24)  NULL,
		[Par]            DECIMAL (18, 2) NOT NULL,
		[Active]              BIT            NULL,
		[CreatedUtc]          DATETIME       DEFAULT (getutcdate()) NOT NULL,
		[ModifiedUtc]         DATETIME       DEFAULT (getutcdate()) NOT NULL,
		PRIMARY KEY CLUSTERED ([Id] ASC)
	)

	INSERT 
		INTO @Detail 
			([ParentCustomListHeaderId], [ItemNumber], [Each], [CatalogId], [Par], [Active]) 
		VALUES 
			(@ParentCustomListHeaderId, @ItemNumber, @Each, @CatalogId, @Par, @Active)

	MERGE INTO [BEK_Commerce_AppData].[List].[CustomListDetails] A
	USING @Detail B ON (A.[ParentCustomListHeaderId] = B.[ParentCustomListHeaderId] and A.[ItemNumber] = B.[ItemNumber] and A.[Each] = B.[Each] and A.[CatalogId] = B.[CatalogId])
	WHEN MATCHED THEN
	    UPDATE SET A.[Par] = B.[Par], A.[Active] = B.[Active], A.[ModifiedUtc] = B.[ModifiedUtc]
	WHEN NOT MATCHED THEN
		INSERT
			([ParentCustomListHeaderId], [ItemNumber], [Each], [CatalogId], [Par]) 
		VALUES 
			(B.[ParentCustomListHeaderId], B.ItemNumber, B.Each, B.CatalogId, B.Par);
		