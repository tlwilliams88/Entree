
CREATE PROCEDURE [ETL].[ReadWorksheetItems]
	@CustomerNumber varchar(10)
	, @DivisionName char(3)
 AS

/*******************************************************************
* PROCEDURE: usp_ECOM_SelectDistinctCustomerContracts
* PURPOSE: Select distinct customer contracts by division
* NOTES: {special set up or requirements, etc.}
* CREATED:	Jason McMillan 9/30/14
* MODIFIED 
* DATE		AUTHOR			DESCRIPTION
*-------------------------------------------------------------------
* {date}	{developer}	{brief modification description}
*******************************************************************/

SELECT
	c.CustomerNumber
	, c.CustomerName
	, LTRIM(RTRIM(cw.ItemNumber)) as ItemNumber
	, cw.BrokenCaseCode
	, cw.ItemPrice
	, cw.QtyOrdered
	, cw.DateOfLastOrder
FROM
	ETL.Staging_Customer c 
	INNER JOIN ETL.Staging_WorksheetItems cw on c.CustomerNumber = cw.CustomerNumber 
		AND c.DIV = cw.DivisionNumber
		AND c.CO = cw.CompanyNumber
		AND c.DEPT = cw.DepartmentNumber
	INNER JOIN ETL.Staging_ItemData i ON LTRIM(RTRIM(cw.ItemNumber)) = LTRIM(RTRIM(i.ItemId)) 
		AND c.DIV = i.BranchId
WHERE
	LTRIM(RTRIM(c.CustomerNumber)) = @CustomerNumber
	AND LOWER(LTRIM(RTRIM(cw.DivisionNumber))) = LOWER(@DivisionName)
	AND i.ItemId NOT LIKE '999%' AND i.SpecialOrderItem <>'Y'
ORDER BY
	cw.ItemNumber ASC


/*
EXEC ETL.usp_ECOM_SelectWorksheetItems '415101', 'FAM'
*/