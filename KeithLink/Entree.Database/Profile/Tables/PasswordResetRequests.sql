CREATE TABLE [Profile].[PasswordResetRequests] (
    [Id]          BIGINT           IDENTITY (1, 1) NOT NULL,
    [UserId]      UNIQUEIDENTIFIER NOT NULL,
    [Token]       NVARCHAR (300)   NULL,
    [Expiration]  DATETIME         NOT NULL,
    [Processed]   BIT              NOT NULL,
    [CreatedUtc]  DATETIME         DEFAULT (getutcdate()) NOT NULL,
    [ModifiedUtc] DATETIME         DEFAULT (getutcdate()) NOT NULL,
    CONSTRAINT [PK_Profile.PasswordResetRequests] PRIMARY KEY CLUSTERED ([Id] ASC)
);

