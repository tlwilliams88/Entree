USE BEK_Commerce_AppData

GO

-- Custom type for passing an array of bigints into stored procedures
CREATE TYPE dbo.BigIntList AS TABLE (Id bigint NOT NULL PRIMARY KEY)

GO
