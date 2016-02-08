-- Extend Order History Detail for UNFI
ALTER TABLE [Orders].[OrderHistoryDetail] ADD [Source] [char](3)
ALTER TABLE [Orders].[OrderHistoryDetail] ADD [ManufacturerId] [nvarchar](25)
ALTER TABLE [Orders].[OrderHistoryDetail] ADD [SpecialOrderHeaderId] [char](7)
ALTER TABLE [Orders].[OrderHistoryDetail] ADD [SpecialOrderLineNumber] [char](3)

-- Merge_UNFI_ExternalCatalog_Mapping_Changes
IF schema_id('Configuration') IS NULL
    EXECUTE('CREATE SCHEMA [Configuration]')
CREATE TABLE [Configuration].[ExternalCatalogs] (
    [Id] [bigint] NOT NULL IDENTITY,
    [BekBranchId] [nvarchar](24),
    [ExternalBranchId] [nvarchar](24),
    [Type] [int] NOT NULL,
    [CreatedUtc] [datetime] NOT NULL DEFAULT GETUTCDATE(),
    [ModifiedUtc] [datetime] NOT NULL DEFAULT GETUTCDATE(),
    CONSTRAINT [PK_Configuration.ExternalCatalogs] PRIMARY KEY ([Id])
)
IF schema_id('Profile') IS NULL
    EXECUTE('CREATE SCHEMA [Profile]')
CREATE TABLE [Profile].[Settings] (
    [Id] [bigint] NOT NULL IDENTITY,
    [UserId] [uniqueidentifier] NOT NULL,
    [Key] [varchar](100) NOT NULL,
    [Value] [varchar](250) NOT NULL,
    [CreatedUtc] [datetime] NOT NULL DEFAULT GETUTCDATE(),
    [ModifiedUtc] [datetime] NOT NULL DEFAULT GETUTCDATE(),
    CONSTRAINT [PK_Profile.Settings] PRIMARY KEY ([Id])
)
CREATE INDEX [IX_UserId] ON [Profile].[Settings]([UserId])
ALTER TABLE [List].[ListItems] ADD [CatalogId] [nvarchar](24)
ALTER TABLE [Orders].[OrderHistoryHeader] ADD [OriginalControlNumber] [char](7)

-- Add Special Order Flag to Order History Header
ALTER TABLE [Orders].[OrderHistoryHeader] ADD [IsSpecialOrder] [bit] NOT NULL DEFAULT 0

-- Add RelatedControlNumber to Order History Header
ALTER TABLE [Orders].[OrderHistoryHeader] ADD [RelatedControlNumber] [char](7)