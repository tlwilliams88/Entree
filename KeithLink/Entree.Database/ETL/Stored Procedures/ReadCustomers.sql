



-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [ETL].[ReadCustomers]
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	SELECT 
		CO AS BranchNumber
		, CustomerNumber
		, CustomerName
		, Address1
		, Address2
		, City
		, [State]
		, ZipCode
		, Telephone
		, SalesRep as DsrNumber
		, ChainStoreCode as NationalOrRegionalAccountNumber
		, [Contract] as ContractNumber
		, PORequiredFlag
		, PowerMenu
		, ContractOnly
		, TermCode
		, CreditLimit
		, CreditHoldFlag
		, DateOfLastPayment
		, AmountDue
		, CurrentBalance
		, PDACXAge1 BalanceAge1
		, PDACXAge2 BalanceAge2
		, PDACXAge3 BalanceAge3
		, PDACXAge4 BalanceAge4
		, AchType
		, DsmNumber
		, NaId NationalId
		, NaNumber NationalNumber
		, NaSub NationalSubNumber
		, RaId RegionalId
		, RaNumber RegionalNumber
		, IsKeithnetCustomer
		, LTRIM(RTRIM(nid.NationalIdDesc)) 'NationalIdDesc'
		, LTRIM(RTRIM(nn.NationalNumberAndSubDesc)) 'NationalNumberAndSubDesc'
		, LTRIM(RTRIM(rid.RegionalIdDesc)) 'RegionalIdDesc'
		, LTRIM(RTRIM(rn.RegionalNumberDesc)) 'RegionalNumberDesc'
	FROM 
		[ETL].Staging_Customer c
		LEFT OUTER JOIN ETL.Staging_NationalIdDesc nid ON LTRIM(RTRIM(c.NaId)) = LTRIM(RTRIM(nid.NationalId))
		LEFT OUTER JOIN ETL.Staging_NationalNumberAndSubDesc nn ON CONCAT(LTRIM(RTRIM(c.NaNumber)), LTRIM(RTRIM(c.NaSub))) = CONCAT(LTRIM(RTRIM(nn.NationalNumber)), LTRIM(RTRIM(nn.NationalSub)))
		LEFT OUTER JOIN ETL.Staging_RegionalIdDesc rid ON LTRIM(RTRIM(c.RaId)) = LTRIM(RTRIM(rid.regionalId))
		LEFT OUTER JOIN ETL.Staging_RegionalNumberDesc rn ON LTRIM(RTRIM(c.RaNumber)) = LTRIM(RTRIM(rn.RegionalNumber))
END