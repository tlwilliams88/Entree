CREATE TABLE [List].[ListItems] (
    [Id]            BIGINT          IDENTITY (1, 1) NOT NULL,
    [ItemNumber]    NVARCHAR (15)   NOT NULL,
    [Label]         NVARCHAR (150)  NULL,
    [Par]           DECIMAL (18, 2) NOT NULL,
    [Note]          NVARCHAR (200)  NULL,
    [CreatedUtc]    DATETIME        DEFAULT (getutcdate()) NOT NULL,
    [ParentList_Id] BIGINT          NULL,
    [ModifiedUtc]   DATETIME        DEFAULT (getutcdate()) NOT NULL,
    [Category]      NVARCHAR (40)   NULL,
    [Position]      INT             DEFAULT ((0)) NOT NULL,
    [FromDate]      DATETIME        NULL,
    [ToDate]        DATETIME        NULL,
    [Each]          BIT             NULL,
    [Quantity]      DECIMAL (18, 2) DEFAULT ((0)) NOT NULL,
    [CatalogId]     NVARCHAR (24)   NULL,
    CONSTRAINT [PK_List.ListItems] PRIMARY KEY CLUSTERED ([Id] ASC) WITH (FILLFACTOR = 80),
    CONSTRAINT [FK_List.ListItems_List.Lists_List_Id] FOREIGN KEY ([ParentList_Id]) REFERENCES [List].[Lists] ([Id])
);


GO
CREATE NONCLUSTERED INDEX [IX_ListItems_ParentList_Id_ItemNumber_Label_Par_Note_CreatedUtc_ModifiedUtc_Category_Position_FromDate_ToDate_Each_Quantity_Cata]
    ON [List].[ListItems]([ParentList_Id] ASC)
    INCLUDE([ItemNumber], [Label], [Par], [Note], [CreatedUtc], [ModifiedUtc], [Category], [Position], [FromDate], [ToDate], [Each], [Quantity], [CatalogId]);


GO
CREATE NONCLUSTERED INDEX [IX_ParentId]
    ON [List].[ListItems]([ParentList_Id] ASC)
    INCLUDE([ItemNumber], [Category], [Position], [Each]) WITH (FILLFACTOR = 80);


GO
CREATE NONCLUSTERED INDEX [IX_ItemParent]
    ON [List].[ListItems]([ItemNumber] ASC, [ParentList_Id] ASC, [Each] ASC) WITH (FILLFACTOR = 80);


GO
CREATE NONCLUSTERED INDEX [IX_ParentList_Id]
    ON [List].[ListItems]([ParentList_Id] ASC) WITH (FILLFACTOR = 80);

