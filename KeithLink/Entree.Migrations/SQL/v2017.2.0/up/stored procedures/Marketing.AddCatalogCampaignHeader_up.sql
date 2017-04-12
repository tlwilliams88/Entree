CREATE PROCEDURE [Marketing].[AddCatalogCampaignHeader]
-- =============================================
-- Author:		Brett Killins
-- Create date: 2/28/2017
-- Description:	<Description,,>
-- =============================================
			@Name          	VARCHAR (250),
			@Description	VARCHAR (5000),
			@StartDate		DATETIME,
			@EndDate		DATETIME,
			@Uri			VARCHAR (250)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	INSERT INTO [Marketing].[CatalogCampaignHeader]
			   ([Name]
			   ,[Description]
			   ,[Active]
			   ,[StartDate]
			   ,[EndDate]
			   ,[Uri])
		 VALUES
			   (@Name
			   ,@Description
			   ,1
			   ,@StartDate
			   ,@EndDate
			   ,@Uri)

    SELECT [Id]
	FROM [Marketing].[CatalogCampaignHeader]
	WHERE [Uri] = @Uri

END