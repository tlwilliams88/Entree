
CREATE PROCEDURE [ETL].[ReadDepartments]
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	SELECT DepartmentId, [ETL].initcap(DepartmentName) AS DepartmentName, ParentDepartment
	FROM [ETL].[Staging_Departments]
END