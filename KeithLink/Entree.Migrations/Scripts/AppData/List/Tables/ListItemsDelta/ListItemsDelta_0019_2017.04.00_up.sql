CREATE TABLE [List].[ListItemsDelta] (
    [Id]            BIGINT        IDENTITY (1, 1) NOT NULL,
    [ItemNumber]    NVARCHAR (15) NOT NULL,
    [Each]          BIT           NULL,
    [ParentList_Id] BIGINT        NULL,
    [CreatedUtc]    DATETIME      DEFAULT (getutcdate()) NOT NULL,
    [ModifiedUtc]   DATETIME      DEFAULT (getutcdate()) NOT NULL,
    [CatalogId]     NVARCHAR (24) NULL,
    [Status]        NVARCHAR (15) NOT NULL,
    [Sent]          BIT           DEFAULT ((0)) NULL
);