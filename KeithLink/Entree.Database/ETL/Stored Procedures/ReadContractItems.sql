
CREATE PROCEDURE [ETL].[ReadContractItems]
	@CustomerNumber varchar(10)
	, @DivisionName char(3)
	, @ContractNumber varchar(10)
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
	, c.ContractOnly
	, LTRIM(RTRIM(c.[Contract])) 'Contract'
	, LTRIM(RTRIM(cb.BidNumber)) 'BidNumber'
	, bh.CompanyNumber
	, bh.DivisionNumber 'DivisionName'
	, bh.DepartmentNumber
	, bh.BidDescription
	, LTRIM(RTRIM(bd.ItemNumber)) 'ItemNumber'
	, bd.BidLineNumber
	, bd.CategoryNumber
	, bd.CategoryDescription
	, CASE
		WHEN ForceEachOrCaseOnly = 'B' THEN 'Y'
		WHEN ForceEachOrCaseOnly = 'C' THEN 'N'
		ELSE 'N'
	END AS 'BrokenCaseCode' --change name and values to match worksheet item query
FROM
	ETL.Staging_Customer c 
	INNER JOIN ETL.Staging_CustomerBid cb on c.CustomerNumber = cb.CustomerNumber 
		AND c.DIV = cb.DivisionNumber
		AND c.CO = cb.CompanyNumber
		AND c.DEPT = DepartmentNumber
	INNER JOIN ETL.Staging_BidContractHeader bh ON cb.BidNumber = bh.BidNumber 
		AND cb.DivisionNumber = bh.DivisionNumber
		AND cb.CompanyNumber = bh.CompanyNumber
		AND cb.DepartmentNumber = bh.DepartmentNumber
	INNER JOIN ETL.Staging_BidContractDetail bd ON bh.BidNumber = bd.BidNumber 
		AND cb.DivisionNumber = bh.DivisionNumber
		AND bh.CompanyNumber = bd.CompanyNumber
		AND bh.DepartmentNumber = bd.DepartmentNumber
WHERE
	LOWER(LTRIM(RTRIM(bh.BidNumber))) = LOWER(@ContractNumber)
	AND LTRIM(RTRIM(c.CustomerNumber)) = @CustomerNumber
	AND LOWER(LTRIM(RTRIM(cb.DivisionNumber))) = LOWER(@DivisionName)
ORDER BY
	bd.BidLineNumber ASC


/*
EXEC ETL.usp_ECOM_SelectContractItems '415101', 'FAM', 'D415101'
*/