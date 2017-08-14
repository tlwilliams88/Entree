CREATE UNIQUE INDEX IX_ContractHeaders_BranchCustomerNumberContractId 
    ON [List].[ContractHeaders] (
        ContractId, 
        BranchId, 
        CustomerNumber
    )
GO