CREATE TABLE [Products].[ClassCodes] (
	ClassCodeId INT NOT NULL PRIMARY KEY,
	ClassCodeName VARCHAR(50) NOT NULL,
	ParentClassCode INT NULL
)