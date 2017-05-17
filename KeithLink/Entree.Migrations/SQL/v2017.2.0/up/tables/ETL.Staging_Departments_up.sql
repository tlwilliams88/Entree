

CREATE TABLE [ETL].[Staging_Departments] (
	[DepartmentId] INT NOT NULL PRIMARY KEY,
	[DepartmentName] VARCHAR(150) NOT NULL,
	[ParentDepartment] INT
)

CREATE UNIQUE INDEX UX_Departments
	ON [ETL].[Staging_Departments] (DepartmentId, ParentDepartment);