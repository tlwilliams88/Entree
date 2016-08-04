
CREATE PROCEDURE ETL.LoadOrgsAndAddressesToCS AS

EXEC ETL.LoadOrganizationsToCS;
EXEC ETL.LoadAddressesToCS;