CREATE PROCEDURE [dbo].[PurgeInternalUserAccess]

AS

TRUNCATE TABLE [Customers].[InternalUserAccess]

RETURN 0