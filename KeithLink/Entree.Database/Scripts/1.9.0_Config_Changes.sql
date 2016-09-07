DECLARE @AppSettings as TABLE 
( 
    [Key]      VARCHAR (50)  NOT NULL,
    [Value]    VARCHAR (MAX) NOT NULL,
    [Comment]  VARCHAR (MAX) NOT NULL,
    [Disabled] BIT           DEFAULT ((0)) NOT NULL,
    PRIMARY KEY CLUSTERED ([Key] ASC)
)

INSERT INTO @AppSettings (Comment, [Key], Value, [Disabled]) VALUES( 'Site Settings', 'SMTPUsername', ' ', 0)
INSERT INTO @AppSettings (Comment, [Key], Value, [Disabled]) VALUES( 'Site Settings', 'SMTPPassword', ' ', 0)
INSERT INTO @AppSettings (Comment, [Key], Value, [Disabled]) VALUES( 'Site Settings', 'SMTPSendPort', '25', 0)
INSERT INTO @AppSettings (Comment, [Key], Value, [Disabled]) VALUES( 'Site Settings', 'ElasticSearchBatchSize', '1000', 0)


MERGE INTO [BEK_Commerce_AppData].[Configuration].[AppSettings] A
USING @AppSettings B ON (A.[Key] = B.[Key])
WHEN MATCHED THEN
    UPDATE SET A.[Comment] = B.[Comment], A.[Value] = B.[Value], A.[Disabled] = B.[Disabled]
WHEN NOT MATCHED THEN
    INSERT (Comment, [Key], Value, [Disabled]) 
	  VALUES(B.[Comment],B.[Key],B.[Value],B.[Disabled]);
