CREATE TABLE [BranchSupport].[Dsrs] (
    [Id]           BIGINT         IDENTITY (1, 1) NOT NULL,
    [DsrNumber]    CHAR (3)       NULL,
    [EmailAddress] VARCHAR (200)  NULL,
    [BranchId]     CHAR (3)       NULL,
    [Name]         NVARCHAR (50)  NULL,
    [Phone]        NVARCHAR (50)  NULL,
    [ImageUrl]     NVARCHAR (200) NULL,
    [CreatedUtc]   DATETIME       DEFAULT (getutcdate()) NOT NULL,
    [ModifiedUtc]  DATETIME       DEFAULT (getutcdate()) NOT NULL,
    CONSTRAINT [PK_BranchSupport.Dsrs] PRIMARY KEY CLUSTERED ([Id] ASC) WITH (FILLFACTOR = 80)
);

