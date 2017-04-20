declare @AppSettings as TABLE 
( 
    [Key]      VARCHAR (50)  NOT NULL,
    [Value]    VARCHAR (MAX) NOT NULL,
    [Comment]  VARCHAR (MAX) NOT NULL,
    [Disabled] BIT           DEFAULT ((0)) NOT NULL,
    PRIMARY KEY CLUSTERED ([Key] ASC)
)

