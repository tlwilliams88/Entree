CREATE TABLE [Customers].[GrowthAndRecoveries](
    [Id] INT NOT NULL IDENTITY,
    [BranchId] VARCHAR(3) NOT NULL,
    [CustomerNumber] VARCHAR(6) NOT NULL,
    [Amount] DECIMAL(10,2) NULL,
    [GrowthAndRecoveryProductGroup] INT NOT NULL,
    [GrowthAndRecoveryTypeKey] INT NOT NULL,
    CONSTRAINT [PK_GrowthAndRecoveries] PRIMARY KEY ([Id])
)
GO