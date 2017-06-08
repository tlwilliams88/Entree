CREATE PROC [List].[DeleteCustomListShare]
    @Id     BIGINT
AS
    -- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    UPDATE
        [List].[CustomListShares]
    SET
        [Active] = 0,
        [ModifiedUtc] = GETUTCDATE()
    WHERE 
        [Id] = @Id
GO
