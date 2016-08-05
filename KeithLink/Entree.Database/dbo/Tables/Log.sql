CREATE TABLE [dbo].[Log] (
    [Id]          INT           IDENTITY (1, 1) NOT NULL,
    [Date]        DATETIME      NOT NULL,
    [Thread]      VARCHAR (255) NOT NULL,
    [Host]        VARCHAR (255) NOT NULL,
    [User]        VARCHAR (255) NOT NULL,
    [Application] VARCHAR (255) NOT NULL,
    [Level]       VARCHAR (50)  NOT NULL,
    [Logger]      VARCHAR (255) NOT NULL,
    [Message]     VARCHAR (MAX) NOT NULL,
    [Exception]   VARCHAR (MAX) NULL
);


GO
CREATE NONCLUSTERED INDEX [IX_Log_Date]
    ON [dbo].[Log]([Date] ASC);


GO
CREATE UNIQUE CLUSTERED INDEX [PK_Log]
    ON [dbo].[Log]([Id] ASC);

