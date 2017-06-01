CREATE PROCEDURE [List].[AddOrUpdateCustomListShareByCustomerNumberBranch] 
	@CustomerNumber	NVARCHAR (10),
	@BranchId		NVARCHAR (10),
	@ParentCustomListHeaderId		BIGINT
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
	  		
		