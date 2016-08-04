CREATE TABLE [Customers].[InternalUserAccess] (
    [Id]             BIGINT           IDENTITY (1, 1) NOT NULL,
    [BranchId]       CHAR (3)         NOT NULL,
    [CustomerNumber] CHAR (6)         NULL,
    [UserId]         UNIQUEIDENTIFIER NOT NULL,
    [CustomerId]     UNIQUEIDENTIFIER NOT NULL,
    [RoleId]         VARCHAR (70)     NOT NULL,
    [EmailAddress]   VARCHAR (200)    NOT NULL,
    [CreatedUtc]     DATETIME         DEFAULT (getutcdate()) NOT NULL,
    [ModifiedUtc]    DATETIME         DEFAULT (getutcdate()) NOT NULL,
    CONSTRAINT [PK_Customers.InternalUserAccess] PRIMARY KEY CLUSTERED ([Id] ASC)
);


GO
CREATE NONCLUSTERED INDEX [IdxByEmailAddress]
    ON [Customers].[InternalUserAccess]([EmailAddress] ASC);


GO
CREATE NONCLUSTERED INDEX [IdxInternalUser]
    ON [Customers].[InternalUserAccess]([BranchId] ASC, [CustomerNumber] ASC) WITH (FILLFACTOR = 80);

