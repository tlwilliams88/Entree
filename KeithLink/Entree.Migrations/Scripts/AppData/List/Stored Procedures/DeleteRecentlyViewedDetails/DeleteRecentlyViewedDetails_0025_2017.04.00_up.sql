CREATE PROCEDURE [List].[DeleteRecentlyViewedDetails] 
    @Id BIGINT
AS
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    DELETE FROM [List].[RecentlyViewedDetails]
    WHERE [Id] = @Id