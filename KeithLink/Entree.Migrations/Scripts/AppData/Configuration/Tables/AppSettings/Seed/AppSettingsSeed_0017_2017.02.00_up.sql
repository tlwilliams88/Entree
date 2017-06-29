declare @AppSettings as TABLE 
( 
    [Key]      VARCHAR (50)  NOT NULL,
    [Value]    VARCHAR (MAX) NOT NULL,
    [Comment]  VARCHAR (MAX) NOT NULL,
    [Disabled] BIT           DEFAULT ((0)) NOT NULL,
    PRIMARY KEY CLUSTERED ([Key] ASC)
)

INSERT 
	INTO @AppSettings 
		([Key], [Value], [Comment], [Disabled]) 
	VALUES 
		('ValidPublicApiTokens', 'db72d0feaf53d708a3f93451bf6dbd1d94a645a7-1e8f-448d-9b76-924dcbea2023', 'Site Settings: Public Api Tokens in CSV format', 0)
		-- add token for Parscales
INSERT 
	INTO @AppSettings 
		([Key], [Value], [Comment], [Disabled]) 
	VALUES 
		('ServePublicApi', 'false', 'Site Settings: true or false as to whether to serve public api', 0)

MERGE INTO [Configuration].[AppSettings] A
USING @AppSettings B ON (A.[Key] = B.[Key])
WHEN MATCHED THEN
    UPDATE SET A.[Comment] = B.[Comment], A.[Value] = B.[Value], A.[Disabled] = B.[Disabled]
WHEN NOT MATCHED THEN
    INSERT (Comment, [Key], Value, [Disabled]) 
	  VALUES(B.[Comment],B.[Key],B.[Value],B.[Disabled]);