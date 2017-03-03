ALTER PROCEDURE [ETL].[ReadProprietaryItems]	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	SELECT
		DISTINCT
		c.CustomerNumber,
		c.DivisionNumber,		
		i.ItemNumber
	FROM
		ETL.Staging_ProprietaryItem i INNER JOIN
		ETL.Staging_ProprietaryCustomer c on i.ProprietaryNumber = c.ProprietaryNumber and i.DepartmentNumber = c.DepartmentNumber
	Order By
		i.ItemNumber
END