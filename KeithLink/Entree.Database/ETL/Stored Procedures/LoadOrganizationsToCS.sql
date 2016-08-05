
CREATE PROCEDURE ETL.LoadOrganizationsToCS AS

SET NOCOUNT ON;

MERGE
	Bek_Commerce_Profiles.dbo.OrganizationObject AS target
USING	
	(	
		SELECT
			LTRIM(RTRIM(CO)) 'BranchNumber'
			, LTRIM(RTRIM(CustomerNumber)) CustomerNumber
			, LTRIM(RTRIM(CustomerName)) CustomerName
			, LTRIM(RTRIM(Address1)) Address1
			, LTRIM(RTRIM(Address2)) Address2
			, LTRIM(RTRIM(City)) City
			, LTRIM(RTRIM([State]))[State] 
			, LTRIM(RTRIM(ZipCode)) ZipCode
			, LTRIM(RTRIM(Telephone)) Telephone
			, LTRIM(RTRIM(SalesRep)) DsrNumber
			, LTRIM(RTRIM(ChainStoreCode)) NationalOrRegionalAccountNumber
			, LTRIM(RTRIM([Contract])) ContractNumber
			, CASE WHEN LTRIM(RTRIM(PORequiredFlag)) = 'Y' THEN 1 ELSE 0 END PORequiredFlag
			, CASE WHEN LTRIM(RTRIM(PowerMenu)) = 'Y' THEN 1 ELSE 0 END PowerMenu
			, LTRIM(RTRIM(ContractOnly)) ContractOnly
			, LTRIM(RTRIM(TermCode)) TermCode
			, LTRIM(RTRIM(CreditLimit)) CreditLimit
			, LTRIM(RTRIM(CreditHoldFlag)) CreditHoldFlag
			, CASE WHEN LTRIM(RTRIM(DateOfLastPayment)) != '00000000' THEN LTRIM(RTRIM(DateOfLastPayment)) ELSE NULL END DateOfLastPayment
			, LTRIM(RTRIM(AmountDue)) AmountDue
			, LTRIM(RTRIM(CurrentBalance)) CurrentBalance
			, LTRIM(RTRIM(PDACXAge1)) BalanceAge1 
			, LTRIM(RTRIM(PDACXAge2)) BalanceAge2
			, LTRIM(RTRIM(PDACXAge3)) BalanceAge3
			, LTRIM(RTRIM(PDACXAge4)) BalanceAge4
			, LTRIM(RTRIM(AchType)) AchType
			, LTRIM(RTRIM(DsmNumber)) DsmNumber
			, LTRIM(RTRIM(NaId)) NationalId
			, LTRIM(RTRIM(NaNumber)) NationalNumber
			, LTRIM(RTRIM(NaSub)) NationalSubNumber
			, LTRIM(RTRIM(RaId)) RegionalId
			, LTRIM(RTRIM(RaNumber)) RegionalNumber
			, LTRIM(RTRIM(IsKeithnetCustomer))
			, LTRIM(RTRIM(nid.NationalIdDesc)) NationalIdDesc
			, LTRIM(RTRIM(nn.NationalNumberAndSubDesc)) NationalNumberAndSubDesc
			, LTRIM(RTRIM(rid.RegionalIdDesc)) RegionalIdDesc
			, LTRIM(RTRIM(rn.RegionalNumberDesc)) RegionalNumberDesc
		FROM 
			Bek_Commerce_AppData.[ETL].Staging_Customer c
			LEFT OUTER JOIN Bek_Commerce_AppData.ETL.Staging_NationalIdDesc nid ON c.NaId = nid.NationalId
			LEFT OUTER JOIN Bek_Commerce_AppData.ETL.Staging_NationalNumberAndSubDesc nn ON CONCAT(c.NaNumber, c.NaSub) = CONCAT(nn.NationalNumber, nn.NationalSub)
			LEFT OUTER JOIN Bek_Commerce_AppData.ETL.Staging_RegionalIdDesc rid ON c.RaId = rid.regionalId
			LEFT OUTER JOIN Bek_Commerce_AppData.ETL.Staging_RegionalNumberDesc rn ON c.RaNumber = rn.RegionalNumber

	)
AS source 
	(
		BranchNumber
		, CustomerNumber
		, CustomerName
		, Address1
		, Address2
		, City
		, [State]
		, ZipCode
		, Telephone
		, DsrNumber
		, NationalOrRegionalAccountNumber
		, ContractNumber
		, PORequiredFlag
		, PowerMenu
		, ContractOnly
		, TermCode
		, CreditLimit
		, CreditHoldFlag
		, DateOfLastPayment
		, AmountDue
		, CurrentBalance
		, BalanceAge1
		, BalanceAge2
		, BalanceAge3
		, BalanceAge4
		, AchType
		, DsmNumber
		, NationalId
		, NationalNumber
		, NationalSubNumber
		, RegionalId
		, RegionalNumber
		, IsKeithnetCustomer
		, NationalIdDesc
		, NationalNumberAndSubDesc
		, RegionalIdDesc
		, RegionalNumberDesc
	)
ON	(
		target.u_customer_number = source.CustomerNumber
		AND target.u_branch_number = source.BranchNumber
	)
WHEN MATCHED THEN
		UPDATE SET		
			u_name = source.CustomerName
			, u_is_po_required = source.PORequiredFlag
			, u_is_power_menu = source.PowerMenu
			, u_contract_number = source.ContractNumber
			, u_dsr_number = source.DsrNumber
			, u_national_or_regional_account_number = source.NationalOrRegionalAccountNumber
			, u_national_account_id = source.NationalId
			, u_term_code = source.TermCode
			, u_credit_limit = source.CreditLimit
			, u_credit_hold_flag = source.CreditHoldFlag
			, u_date_of_last_payment = source.DateOfLastPayment
			, u_amount_due = source.AmountDue
			, u_current_balance = source.CurrentBalance
			, u_balance_age_1 = source.BalanceAge1
			, u_balance_age_2 = source.BalanceAge2
			, u_balance_age_3 = source.BalanceAge3
			, u_balance_age_4 = source.BalanceAge4
			, u_customer_ach_type = source.AchType
			, u_dsm_number = source.DsmNumber
			, u_national_id = source.NationalId
			, u_national_number = source.NationalNumber
			, u_national_sub_number = source.NationalSubNumber
			, u_regional_id = source.RegionalId
			, u_regional_number = source.RegionalNumber
			, u_is_keithnet_customer = source.IsKeithnetCustomer
			, u_national_id_desc = source.NationalIdDesc
			, u_national_numbersub_desc = source.NationalNumberAndSubDesc
			, u_regional_id_desc = source.RegionalIdDesc
			, u_regional_number_desc = source.RegionalNumberDesc

WHEN NOT MATCHED THEN	
	INSERT	
		(
			u_org_id
			, u_customer_number
			, u_branch_number
			, u_name
			, u_is_po_required
			, u_is_power_menu
			, u_contract_number
			, u_dsr_number
			, u_national_or_regional_account_number
			, u_organization_type
			, u_national_account_id
			, u_term_code
			, u_credit_limit
			, u_credit_hold_flag
			, u_date_of_last_payment
			, u_amount_due
			, u_current_balance
			, u_balance_age_1
			, u_balance_age_2
			, u_balance_age_3
			, u_balance_age_4
			, u_customer_ach_type
			, u_dsm_number
			, u_national_id
			, u_national_number
			, u_national_sub_number
			, u_regional_id
			, u_regional_number
			, u_is_keithnet_customer
			, u_national_id_desc
			, u_national_numbersub_desc
			, u_regional_id_desc
			, u_regional_number_desc
		)
	VALUES
		(
			CONCAT('{',lower(newid()),'}')
			, source.CustomerNumber
			, source.BranchNumber
			, source.CustomerName
			, source.PORequiredFlag
			, source.PowerMenu
			, source.ContractNumber
			, source.DsrNumber
			, source.NationalOrRegionalAccountNumber
			, 0 --org type
			, source.NationalId
			, source.TermCode
			, source.CreditLimit
			, source.CreditHoldFlag
			, source.DateOfLastPayment
			, source.AmountDue
			, source.CurrentBalance
			, source.BalanceAge1
			, source.BalanceAge2
			, source.BalanceAge3
			, source.BalanceAge4
			, source.AchType
			, source.DsmNumber
			, source.NationalId
			, source.NationalNumber
			, source.NationalSubNumber
			, source.RegionalId
			, source.RegionalNumber
			, source.IsKeithnetCustomer
			, source.NationalIdDesc
			, source.NationalNumberAndSubDesc
			, source.RegionalIdDesc
			, source.RegionalNumberDesc
		);