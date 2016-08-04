CREATE TABLE [Profile].[MarketingPreferences] (
    [Id]              BIGINT         IDENTITY (1, 1) NOT NULL,
    [Email]           NVARCHAR (150) NULL,
    [BranchId]        NVARCHAR (MAX) NULL,
    [CurrentCustomer] BIT            NOT NULL,
    [LearnMore]       BIT            NOT NULL,
    [RegisteredOn]    DATETIME       NOT NULL,
    [CreatedUtc]      DATETIME       DEFAULT (getutcdate()) NOT NULL,
    [ModifiedUtc]     DATETIME       DEFAULT (getutcdate()) NOT NULL,
    CONSTRAINT [PK_Profile.MarketingPreferences] PRIMARY KEY CLUSTERED ([Id] ASC)
);

