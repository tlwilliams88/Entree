CREATE PROC [List].[DeleteCustomListDetails]
    @Id     BIGINT
AS
    -- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    UPDATE
        [List].[CustomListDetails]
    SET
        [Active] = 0,
        [ModifiedUtc] = GETUTCDATE()
    WHERE 
        [Id] = @Id
GO
