CREATE PROCEDURE [Marketing].[UpdateCatalogCampaignItemByHeader]
-- =============================================
-- Author:		Brett Killins
-- Create date: 2/28/2017
-- Description:	<Description,,>
-- =============================================
			@ItemNumber 				VARCHAR (6),
			@CatalogCampaignHeaderId	BIGINT,
			@Active						BIT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	UPDATE [Marketing].[CatalogCampaignItems]
	SET [Active] = @Active
	WHERE [CatalogCampaignHeaderId] = @CatalogCampaignHeaderId
		AND [ItemNumber] = @ItemNumber

END