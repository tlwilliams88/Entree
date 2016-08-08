
CREATE PROCEDURE ETL.LoadOrgsAndAddressesToCS AS

SET NOCOUNT ON

EXEC ETL.LoadOrganizationsToCS;
EXEC ETL.LoadAddressesToCS;