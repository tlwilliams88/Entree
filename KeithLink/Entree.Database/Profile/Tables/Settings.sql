CREATE TABLE [Profile].[Settings] (
    [Id]          BIGINT           IDENTITY (1, 1) NOT NULL,
    [UserId]      UNIQUEIDENTIFIER NOT NULL,
    [Key]         VARCHAR (100)    NOT NULL,
    [Value]       VARCHAR (250)    NOT NULL,
    [CreatedUtc]  DATETIME         DEFAULT (getutcdate()) NOT NULL,
    [ModifiedUtc] DATETIME         DEFAULT (getutcdate()) NOT NULL,
    CONSTRAINT [PK_Profile.Settings] PRIMARY KEY CLUSTERED ([Id] ASC)
);


GO
CREATE NONCLUSTERED INDEX [IX_UserId]
    ON [Profile].[Settings]([UserId] ASC);

