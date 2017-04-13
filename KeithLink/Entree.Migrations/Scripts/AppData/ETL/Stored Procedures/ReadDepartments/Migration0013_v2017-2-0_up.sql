
CREATE PROCEDURE [ETL].[ReadDepartments]
AS
/**
*
* -------------------------------------------------------
* Read the item data for the elastic search load. 
* This proc is used by the internalsvc load process.
* -------------------------------------------------------
*   Changed  | By
* -------------------------------------------------------
* 2017-03-03 | mdjoiner
*
**/
BEGIN
	SET NOCOUNT ON;
	SELECT DepartmentId, [ETL].initcap(DepartmentName) AS DepartmentName, ParentDepartment
	FROM [ETL].[Staging_Departments]
END