CREATE PROCEDURE [Marketing].[AddCatalogCampaignItemByHeader]
-- =============================================
-- Author:		Brett Killins
-- Create date: 2/28/2017
-- Description:	<Description,,>
-- =============================================
			@ItemNumber 				VARCHAR (6),
			@CatalogCampaignHeaderId	BIGINT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	INSERT INTO [Marketing].[CatalogCampaignItems]
			   ([ItemNumber]
			   ,[CatalogCampaignHeaderId]
			   ,[Active])
		 VALUES
			   (@ItemNumber
			   ,@CatalogCampaignHeaderId
			   ,1)

END