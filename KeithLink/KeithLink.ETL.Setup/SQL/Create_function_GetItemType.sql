USE FSE;
GO

/*******************************************************************
* FUNCTION: GetItemType
* PURPOSE: Selects the appropriate item type, giving the Gtin number
* NOTES: {special set up or requirements, etc.}
* CREATED:	Jason McMillan 7-25-14
* MODIFIED 
* DATE		AUTHOR			DESCRIPTION
*-------------------------------------------------------------------
* {date}	{developer}	{brief modification description}
*******************************************************************/

CREATE FUNCTION dbo.GetItemType 
	(
		@Gtin char(14)
	)
RETURNS varchar(20)
BEGIN
	DECLARE @packageType AS varchar(20);

	DECLARE @GtinIdentifier char(1);
	SET @GtinIdentifier = CAST(LEFT(@Gtin,1) AS int);

	IF @GtinIdentifier = 0 
		SET @packageType = 'pack'
	ELSE IF @GtinIdentifier > 0 AND @GtinIdentifier < 9
		SET @packageType = 'case'
	ELSE IF @GtinIdentifier = 9 
		SET @packageType = 'catch_weight'
	ELSE
		SET @packageType = 'invalid_gtin_identifier'

	RETURN @packageType;
END


 