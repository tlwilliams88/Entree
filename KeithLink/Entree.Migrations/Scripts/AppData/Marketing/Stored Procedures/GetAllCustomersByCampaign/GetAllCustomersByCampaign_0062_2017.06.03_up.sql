CREATE PROC Marketing.GetAllCustomersByCampaign 
    @CampaignId INT
AS
    SELECT
        cc.CampaignId,
        cc.BranchId,
        cc.CustomerNumber
    FROM
        Marketing.CampaignCustomers cc
    WHERE
        cc.CampaignId = @CampaignId