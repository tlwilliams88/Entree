CREATE TABLE [ETL].[Staging_EmployeeInfo] (
    [EMPLID]         VARCHAR (11)    NOT NULL,
    [EMAIL_ADDR]     VARCHAR (70)    NULL,
    [PHONE]          VARCHAR (24)    NULL,
    [EMPLOYEE_PHOTO] VARBINARY (MAX) NULL,
    [LAST_NAME]      VARCHAR (30)    NULL,
    [FIRST_NAME]     VARCHAR (30)    NULL
);

