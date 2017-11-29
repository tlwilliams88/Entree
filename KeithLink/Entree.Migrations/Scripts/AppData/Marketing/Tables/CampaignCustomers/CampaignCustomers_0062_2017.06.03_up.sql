CREATE TABLE Marketing.CampaignCustomers (
    CampaignId      INT,
    BranchId        CHAR(3),
    CustomerNumber  CHAR(6),
    PRIMARY KEY(CampaignId, BranchId, CustomerNumber)
)