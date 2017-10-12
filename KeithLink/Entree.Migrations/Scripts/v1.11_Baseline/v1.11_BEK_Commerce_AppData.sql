/****** Object:  Schema [BranchSupport]    Script Date: 10/27/2016 1:05:24 PM ******/
CREATE SCHEMA [BranchSupport]
GO
/****** Object:  Schema [Configuration]    Script Date: 10/27/2016 1:05:24 PM ******/
CREATE SCHEMA [Configuration]
GO
/****** Object:  Schema [ContentManagement]    Script Date: 10/27/2016 1:05:24 PM ******/
CREATE SCHEMA [ContentManagement]
GO
/****** Object:  Schema [Customers]    Script Date: 10/27/2016 1:05:24 PM ******/
CREATE SCHEMA [Customers]
GO
/****** Object:  Schema [ETL]    Script Date: 10/27/2016 1:05:24 PM ******/
CREATE SCHEMA [ETL]
GO
/****** Object:  Schema [Invoice]    Script Date: 10/27/2016 1:05:24 PM ******/
CREATE SCHEMA [Invoice]
GO
/****** Object:  Schema [List]    Script Date: 10/27/2016 1:05:24 PM ******/
CREATE SCHEMA [List]
GO
/****** Object:  Schema [Messaging]    Script Date: 10/27/2016 1:05:24 PM ******/
CREATE SCHEMA [Messaging]
GO
/****** Object:  Schema [Orders]    Script Date: 10/27/2016 1:05:24 PM ******/
CREATE SCHEMA [Orders]
GO
/****** Object:  Schema [Profile]    Script Date: 10/27/2016 1:05:24 PM ******/
CREATE SCHEMA [Profile]
GO
/****** Object:  StoredProcedure [Configuration].[CheckAppSettingsForChange]    Script Date: 10/27/2016 1:05:24 PM ******/
SET ANSI_NULLS OFF
GO
SET QUOTED_IDENTIFIER OFF
GO
 CREATE PROCEDURE [Configuration].[CheckAppSettingsForChange]
 AS
 BEGIN
 SET NOCOUNT ON;
   SELECT 
     Value
   FROM 
     [Configuration].[AppSettings]
   WHERE
       [Key] = 'DBChangeValue'
 END

GO
/****** Object:  StoredProcedure [Configuration].[GetAppSetting]    Script Date: 10/27/2016 1:05:24 PM ******/
SET ANSI_NULLS OFF
GO
SET QUOTED_IDENTIFIER OFF
GO
CREATE PROCEDURE [Configuration].[GetAppSetting]
	@Key		varchar(50)
AS
	SELECT 
 		[Key], 
		Value,
		Comment,
		[Disabled]
 	FROM 
 		[Configuration].[AppSettings]
 	WHERE
 	    [Key] = @Key

GO
/****** Object:  StoredProcedure [Configuration].[ReadAppSettings]    Script Date: 10/27/2016 1:05:24 PM ******/
SET ANSI_NULLS OFF
GO
SET QUOTED_IDENTIFIER OFF
GO
 CREATE PROCEDURE [Configuration].[ReadAppSettings]
 AS
 BEGIN
 SET NOCOUNT ON;
 	SELECT 
 		[Key], 
		Value,
		Comment,
		[Disabled]
 	FROM 
 		[Configuration].[AppSettings]
 	WHERE
 	    [Disabled] = 0
 END

GO
/****** Object:  StoredProcedure [Configuration].[SaveAppSetting]    Script Date: 10/27/2016 1:05:24 PM ******/
SET ANSI_NULLS OFF
GO
SET QUOTED_IDENTIFIER OFF
GO
CREATE PROCEDURE [Configuration].[SaveAppSetting]
	@Key		VARCHAR(50),
	@Value		VARCHAR(MAX),
	@Comment	VARCHAR(MAX),
	@Disabled	BIT
AS
	UPDATE	Configuration.AppSettings
	SET
		[Value] = @Value,
		[Comment] = @Comment,
		[Disabled] = @Disabled
	WHERE
		[Key] = @Key

GO
/****** Object:  StoredProcedure [dbo].[spCreateAuditLogEntry]    Script Date: 10/27/2016 1:05:24 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[spCreateAuditLogEntry]
	@Type int,
	@TypeDescription nvarchar(50),
	@Actor nvarchar(100) = null,
	@Information nvarchar(max) = null
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    INSERT INTO [dbo].[AuditLog]
           ([Type]
           ,[TypeDescription]
           ,[Actor]
           ,[Information])
     VALUES
           (@Type
           ,@TypeDescription
           ,@Actor
           ,@Information)
END



GO
/****** Object:  StoredProcedure [ETL].[LoadAddressesToCS]    Script Date: 10/27/2016 1:05:24 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [ETL].[LoadAddressesToCS] AS

SET NOCOUNT ON;

DECLARE @stagingAddresses TABLE
(
	CustomerNumber varchar(255)
	, BranchId varchar(255)
	, Address1 varchar(255)
	, Address2 varchar(255)
	, City varchar(255)
	, [State] varchar(255)
	, Zip varchar(255)
	, Telephone varchar(255)
)
INSERT INTO @stagingAddresses
SELECT
	LTRIM(RTRIM(CustomerNumber)) 
	, LTRIM(RTRIM(CO)) 
	, LTRIM(RTRIM(Address1)) 
	, LTRIM(RTRIM(Address2)) 
	, LTRIM(RTRIM(City)) 
	, LTRIM(RTRIM([State]))
	, LTRIM(RTRIM(ZipCode))
	, LTRIM(RTRIM(Telephone))
FROM
	Bek_Commerce_AppData.ETL.Staging_Customer

MERGE Bek_Commerce_Profiles..Addresses AS target
USING
	(
		SELECT
			Address1
			, Address2
			, City
			, [State]
			, Zip
			, Telephone
		FROM
			@stagingAddresses
	) AS source
	(
		Address1
			, Address2
			, City
			, [State]
			, Zip
			, Telephone
	)
ON 
	(
		target.u_address_line1 = source.Address1
		AND target.u_postal_code = source.Zip
		AND target.u_tel_number = source.Telephone
	)
WHEN NOT MATCHED THEN
	INSERT
	(
		u_address_id
		, u_address_name
		, u_address_line1
		, u_address_line2
		, u_city
		, u_region_code
		, u_postal_code
		, u_tel_number
	)
	VALUES
	(
		CONCAT('{',lower(NEWID()),'}')
		, ''
		, source.Address1
		, source.Address2
		, source.City
		, source.[State]
		, source.Zip
		, source.Telephone
	);

DECLARE c CURSOR FOR
SELECT
	CustomerNumber
	, BranchId
	, Address1
	, Zip
	, Telephone
FROM
	@stagingAddresses

DECLARE @CustomerNumber varchar(255);
DECLARE @BranchId varchar(255);
DECLARE @Address1 varchar(255);
DECLARE @Zip varchar(255);
DECLARE @Telephone varchar(255);

OPEN c;

FETCH NEXT FROM c INTO @CustomerNumber, @BranchId, @Address1, @Zip, @Telephone;

WHILE @@FETCH_STATUS = 0
BEGIN
	DECLARE @AddressId varchar(255);
	SET @AddressId = (
		SELECT
			TOP 1
			u_address_id
		FROM	
			Bek_Commerce_Profiles..Addresses
		WHERE
			u_address_line1 = @Address1
			AND u_postal_code = @Zip
			AND u_tel_number = @Telephone
	);
	
	UPDATE 
		Bek_Commerce_Profiles..OrganizationObject
	SET		
		u_preferred_address = @AddressId
	WHERE
		u_customer_number = @CustomerNumber
		AND u_branch_number = @BranchId

	FETCH NEXT FROM c INTO @CustomerNumber, @BranchId, @Address1, @Zip, @Telephone;
END
CLOSE c;
DEALLOCATE c;



GO
/****** Object:  StoredProcedure [ETL].[LoadOrganizationsToCS]    Script Date: 10/27/2016 1:05:24 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [ETL].[LoadOrganizationsToCS] AS

SET NOCOUNT ON;

MERGE
	Bek_Commerce_Profiles..OrganizationObject AS target
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



GO
/****** Object:  StoredProcedure [ETL].[LoadOrgsAndAddressesToCS]    Script Date: 10/27/2016 1:05:24 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [ETL].[LoadOrgsAndAddressesToCS] AS

SET NOCOUNT ON

EXEC ETL.LoadOrganizationsToCS;
EXEC ETL.LoadAddressesToCS;

GO
/****** Object:  StoredProcedure [ETL].[ProcessBEKItemNumbersForUNFIByVendorId]    Script Date: 10/27/2016 1:05:24 PM ******/
SET ANSI_NULLS OFF
GO
SET QUOTED_IDENTIFIER OFF
GO
/*
* Retieve BEK ItemNumber by VendorId using External Catalogs for branch
* 
* Author: mdjoiner
* Changed: 2016-05-13
*/
CREATE PROCEDURE [ETL].[ProcessBEKItemNumbersForUNFIByVendorId]
    @vendorId varchar(10)
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE
        unfi
    SET
        unfi.ProductNumber = item.ItemId,
        unfi.StockedInBranches = ETL.ConcatBranchesForItem(@vendorId, item.ItemId)
    FROM
        ETL.Staging_UNFIProducts unfi
        JOIN ETL.Staging_ItemData item ON item.MfrNumber = unfi.ProductNumber
    WHERE
        item.Vendor1 = @vendorId
    AND
        item.SpecialOrderItem = 'N'
END

GO
/****** Object:  StoredProcedure [ETL].[ProcessContractItemList]    Script Date: 10/27/2016 1:05:24 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Joshua P. Tirey
-- Create date: 3/29/2015
-- Description:	Creates Entree List for Customer Contract items
-- =============================================
CREATE PROCEDURE [ETL].[ProcessContractItemList] 
AS
BEGIN

SET NOCOUNT ON

DECLARE @customerId varchar(15)
DECLARE @contractNumber varchar(50)
DECLARE @branchID varchar(50)
DECLARE @CurrentBidNumber varchar(15)
DECLARE @CurrentDivision varchar(15)
DECLARE @countT int
DECLARE @existingListId bigint
DECLARE @existingAddedListId bigint
DECLARE @existingDeletedListId bigint

DECLARE @rowCount int
Set @rowCount = 0

SET @CurrentBidNumber = ''
SET @CurrentDivision = ''

DECLARE @TempContractItems TABLE
(
	ItemNumber varchar(10),
	BidNumber varchar(10),
	DivisionNumber varchar(10),
	Each bit,
	BidLineNumber int,
	CategoryDescription varchar(40)
)

DECLARE @AddedItems TABLE
(
	ItemNumber varchar(10),
	Each bit,
	CategoryDescription varchar(100),
	BidLineNumber int
)
DECLARE @DeletedItems TABLE
(
	ItemNumber varchar(10),
	Each bit,
	CategoryDescription varchar(100),
	BidLineNumber int
)

--DECLARE Cursor for all contracts
DECLARE contract_Cursor CURSOR FAST_FORWARD FOR
	SELECT 
		[CustomerNumber]
		,[BidNumber]
		,[DivisionNumber]
	FROM 
		[BEK_Commerce_AppData].[ETL].[Staging_CustomerBid]
	ORDER BY
		BidNumber, DivisionNumber


OPEN contract_Cursor
FETCH NEXT FROM contract_Cursor INTO @customerId, @contractNumber, @branchID

WHILE @@FETCH_STATUS = 0
BEGIN
	Print @rowCount

	IF (@contractNumber <> @CurrentBidNumber OR @branchID <> @CurrentDivision)
	BEGIN		
		DELETE FROM  @TempContractItems
		INSERT INTO @TempContractItems (ItemNumber, BidNumber, DivisionNumber, Each, BidLineNumber, CategoryDescription)
		SELECT 
				LTRIM(RTRIM(d.ItemNumber)),
				LTRIM(RTRIM(d.BidNumber)),
				LTRIM(RTRIM(d.DivisionNumber)),
				CASE WHEN ForceEachOrCaseOnly = 'B' THEN 1 ELSE 0 END AS 'BrokenCaseCode',
				BidLineNumber,
				CategoryDescription
			FROM
				[BEK_Commerce_AppData].[ETL].[Staging_BidContractDetail] d
			WHERE
				BidNumber = @contractNumber AND
				DivisionNumber = @branchID
		
		
		SET @CurrentBidNumber = @contractNumber
		SET @CurrentDivision = @branchID
	END

	
	IF NOT EXISTS(SELECT 'x' FROM @TempContractItems) --No items for this contract, continue to next
	BEGIN
		GOTO cont
	END

	--Find existing contract list for the customer
	SELECT @existingListId = Id from [BEK_Commerce_AppData].[List].Lists WHERE Type = 2 AND CustomerId = LTRIM(RTRIM(@customerId)) AND BranchId = LTRIM(RTRIM(@branchID))
	Print @existingListId
	IF @existingListId IS NULL
		BEGIN
			--List doesn't exist -- Create list
			INSERT INTO [BEK_Commerce_AppData].[List].[Lists]
				([DisplayName]
				,[Type]
				,[CustomerId]
				,[BranchId]
				,[ReadOnly]
				,[CreatedUtc]
				,[ModifiedUtc])
			VALUES
				('Contract - ' + LTRIM(RTRIM(@contractNumber))
				,2
				,LTRIM(RTRIM(@customerId))
				,LTRIM(RTRIM(@branchID))
				,1
				,GETUTCDATE()
				,GETUTCDATE())

			SET @existingListId = SCOPE_IDENTITY();

			--Insert items into the new list
			INSERT INTO [BEK_Commerce_AppData].[List].[ListItems]
					   ([ItemNumber]
					   ,[Par]
					   ,[CreatedUtc]
					   ,[ParentList_Id]
					   ,[ModifiedUtc]
					   ,[Category]
					   ,[Position]
					   ,[Each]
					   ,[CatalogId])
			SELECT 
				LTRIM(RTRIM(ItemNumber)) 'ItemNumber'
				 ,0.00
				,GETUTCDATE()
				, @existingListId
				, GETUTCDATE()
				, CategoryDescription
				, BidLineNumber
				, Each	
				, LTRIM(RTRIM(@branchID))
			FROM 
				@TempContractItems
			ORDER BY
				BidLineNumber ASC

		END 
	ELSE
		BEGIN
			--List already exist. Update with new or deleted items; update contract number.
			UPDATE
				[List].Lists
			SET
				DisplayName = (SELECT CONCAT('Contract - ', LTRIM(RTRIM(@contractNumber)))) 
			WHERE
				[Id] = @existingListId
				
			--update category on already existing lineitem itemnumbers
			update li
				set li.Category=bd.CategoryDescription
				from List.ListItems as li
				inner join List.Lists as l
					on l.Id=li.ParentList_Id
				inner join etl.Staging_CustomerBid cb
					on l.CustomerId=ltrim(rtrim(cb.CustomerNumber)) and l.BranchId = ltrim(rtrim(cb.divisionnumber))
				inner join etl.Staging_BidContractDetail bd
					on ltrim(rtrim(cb.BidNumber))=ltrim(rtrim(bd.BidNumber)) and li.ItemNumber=LTRIM(rtrim(bd.itemnumber))
				where l.[Id] = @existingListId

			--Find new items to be added
			INSERT INTO @AddedItems (ItemNumber, Each, CategoryDescription, BidLineNumber)
			SELECT 
				d.ItemNumber, 
				Each,
				CategoryDescription,
				BidLineNumber
			FROM
				@TempContractItems d
			WHERE 
				NOT EXISTS(SELECT 'x' FROM [BEK_Commerce_AppData].[List].ListItems li 
							WHERE li.ItemNumber = LTRIM(RTRIM(d.ItemNumber)) AND li.Each = d.Each AND li.ParentList_Id = @existingListId)

			--Find items being deleted
			INSERT INTO @DeletedItems (ItemNumber, Each, CategoryDescription, BidLineNumber)
			SELECT
				l.ItemNumber,
				l.Each,
				l.Category,
				l.Position
			FROM
				[BEK_Commerce_AppData].[List].[ListItems] l
			WHERE
				l.ParentList_Id = @existingListId AND
				NOT EXISTS(SELECT 
						'x'
					FROM
						@TempContractItems d
					WHERE
						LTRIM(RTRIM(d.ItemNumber)) = l.ItemNumber AND
						d.Each = l.Each)							

			--New items to add?
			IF EXISTS(SELECT 'x' FROM @AddedItems)
				BEGIN
					--Add to a Contract Added List, create if one doesn't already exist
					
					SELECT @existingAddedListId = Id from [BEK_Commerce_AppData].[List].Lists WHERE Type = 6 AND CustomerId = @customerId AND BranchId = @branchID
					
					Print 'Existing added list' 
					Print @existingAddedListId

					IF @existingAddedListId IS NULL
					BEGIN
						Print 'Creating List'
						INSERT INTO [BEK_Commerce_AppData].[List].[Lists]
							([DisplayName]
							,[Type]
							,[CustomerId]
							,[BranchId]
							,[ReadOnly]
							,[CreatedUtc]
							,[ModifiedUtc])
						VALUES
							('Contract Items Added'
							,6
							,@customerId
							,@branchID
							,1
							,GETUTCDATE()
							,GETUTCDATE())

						SET @existingAddedListId = SCOPE_IDENTITY();
					END
					
					--Insert Items into the Contracted Added list
					INSERT INTO [BEK_Commerce_AppData].[List].[ListItems]
							   ([ItemNumber]
							   ,[Par]
							   ,[CreatedUtc]
							   ,[ParentList_Id]
							   ,[ModifiedUtc]
							   ,[Category]
							   ,[Position]
							   ,[Each]
							   ,[CatalogId])
					SELECT
						LTRIM(RTRIM(ItemNumber)),
						0.00,
						GETUTCDATE(),
						@existingAddedListId,
						GETUTCDATE(),
						CategoryDescription,
						BidLineNumber,
						Each,
						@branchID
					FROM
						@AddedItems

					--Insert items into the list
					INSERT INTO [BEK_Commerce_AppData].[List].[ListItems]
							   ([ItemNumber]
							   ,[Par]
							   ,[CreatedUtc]
							   ,[ParentList_Id]
							   ,[ModifiedUtc]
							   ,[Category]
							   ,[Position]
							   ,[Each]
							   ,[CatalogId])
					SELECT
						LTRIM(RTRIM(ItemNumber)),
						0.00,
						GETUTCDATE(),
						@existingListId,
						GETUTCDATE(),
						CategoryDescription,
						BidLineNumber,
						Each,
						@branchID
					FROM
						@AddedItems

				END
				
			--Items to delete
			IF EXISTS(SELECT 'x' FROM @DeletedItems)
				BEGIN
					--Add to a Contract Added List, create if one doesn't already exist
					
					SELECT @existingDeletedListId = Id from [BEK_Commerce_AppData].[List].Lists WHERE Type = 7 AND CustomerId = @customerId AND BranchId = @branchID
										
					IF @existingDeletedListId IS NULL
					BEGIN
						INSERT INTO [BEK_Commerce_AppData].[List].[Lists]
							([DisplayName]
							,[Type]
							,[CustomerId]
							,[BranchId]
							,[ReadOnly]
							,[CreatedUtc]
							,[ModifiedUtc])
						VALUES
							('Contract Items Deleted'
							,7
							,@customerId
							,@branchID
							,1
							,GETUTCDATE()
							,GETUTCDATE())

						SET @existingDeletedListId = SCOPE_IDENTITY();
					END


					--DELETE Item
					DELETE [BEK_Commerce_AppData].[List].[ListItems]
					FROM [BEK_Commerce_AppData].[List].[ListItems] li inner join
						@DeletedItems d on li.ItemNumber = d.ItemNumber AND li.Each = d.Each and ParentList_Id = @existingListId
					
					--Insert Items into the Contracted Deleted list
					INSERT INTO [BEK_Commerce_AppData].[List].[ListItems]
							   ([ItemNumber]
							   ,[Par]
							   ,[CreatedUtc]
							   ,[ParentList_Id]
							   ,[ModifiedUtc]
							   ,[Category]
							   ,[Position]
							   ,[Each]
							   ,[CatalogId])
					SELECT
						LTRIM(RTRIM(ItemNumber)),
						0.00,
						GETUTCDATE(),
						@existingDeletedListId,
						GETUTCDATE(),
						CategoryDescription,
						BidLineNumber,
						Each,
						@branchID
					FROM
						@DeletedItems
				END				
			
		END

	
cont:
	
	SET @existingListId = null
	SET @existingAddedListId = null
	SET @existingDeletedListId = null
	DELETE FROM @AddedItems
	DELETE FROM @DeletedItems
	SET @rowCount = @rowCount +1
	FETCH NEXT FROM contract_Cursor INTO @customerId, @contractNumber, @branchID
END

close contract_Cursor
DEALLOCATE contract_Cursor
END
GO
/****** Object:  StoredProcedure [ETL].[ProcessItemHistoryData]    Script Date: 10/27/2016 1:05:24 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [ETL].[ProcessItemHistoryData]       
       @NumWeeks int
AS

SET NOCOUNT ON;

TRUNCATE TABLE [Customers].[ItemHistory];

BEGIN TRANSACTION

INSERT INTO
	[Customers].[ItemHistory]
	(
		BranchId
		, CustomerNumber
		, ItemNumber
		, CreatedUtc
		, ModifiedUtc
		, UnitOfMeasure
		, AverageUse
	)
SELECT
	oh.BranchId
	, oh.CustomerNumber
	, od.ItemNumber
	, GETUTCDATE()
	, GETUTCDATE()
	, od.unitOfMeasure
	, AVG(od.ShippedQuantity)
FROM 
	Orders.OrderHistoryHeader oh
	INNER JOIN Orders.OrderHistoryDetail od ON od.OrderHistoryHeader_Id = oh.Id
WHERE 
	CONVERT(DATE, oh.CreatedUtc) > DATEADD(ww, (@NumWeeks * -1), CONVERT(DATE, GETDATE()))
GROUP BY 
	oh.BranchId
	, oh.CustomerNumber
	, od.ItemNumber
	, od.unitOfMeasure

IF @@ERROR = 0 
	COMMIT TRANSACTION
ELSE	
	ROLLBACK TRANSACTION



GO
/****** Object:  StoredProcedure [ETL].[ProcessStagedInvoices]    Script Date: 10/27/2016 1:05:24 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [ETL].[ProcessStagedInvoices]
AS
BEGIN
	
	SET NOCOUNT ON

	CREATE TABLE #NewInvoices (InvoiceNumber varchar(20))
	CREATE TABLE #OldInvoices (InvoiceNumber varchar(20))

	/*************************************************************************
	/
	/	Populate Terms table
	/
	**************************************************************************/
	--Terms is just a lookup table, for now just empty and repopulate
	DELETE [Invoice].[Terms]

	INSERT INTO [Invoice].[Terms] (BranchId, TermCode, [Description], Age1, Age2, Age3, Age4, CreatedUtc, ModifiedUtc)
		SELECT
			Company,
			CONVERT(INTEGER, Code),
			[Description],
			CONVERT(INTEGER, Age1),
			CONVERT(INTEGER, Age2),
			CONVERT(INTEGER, Age3),
			CONVERT(INTEGER, Age4),
			GETUTCDATE(),
			GETUTCDATE()
		FROM
			[ETL].[Staging_Terms]



	/*************************************************************************
	/
	/	Create new invoices
	/
	**************************************************************************/
	--Save invoice Ids that are in today's order detail file and weren't previously there
	INSERT INTO #NewInvoices (InvoiceNumber)
		SELECT 
			DISTINCT o.InvoiceNumber 
		FROM
			[ETL].Staging_KNet_Invoice i inner join
			[ETL].Staging_OpenDetailAR o on i.InvoiceNumber = o.InvoiceNumber
		WHERE
			o.InvoiceNumber NOT IN (SELECT InvoiceNumber FROM [Invoice].[Invoices])
		

	--INSERT Header entries
	INSERT INTO [Invoice].[Invoices] (InvoiceNumber, OrderDate, InvoiceDate, CustomerNumber, CreatedUtc, ModifiedUtc, BranchId, [Type], Amount, [Status], DueDate)
		SELECT
			DISTINCT
			o.InvoiceNumber,
			CONVERT(DATETIME, i.OrderDate),
			CONVERT(DATETIME, i.ShipDate),
			o.Customer,
			GETUTCDATE(),
			GETUTCDATE(),
			o.Company,
			CASE 
				WHEN o.InvoiceNumber LIKE '%-C-%' THEN 1 
				WHEN o.InvoiceNumber LIKE '%-A-%' THEN 2
				ELSE 0
			END,
			CONVERT(decimal(18,2), o.InvoiceAmount),
			0, --Open
			CONVERT(DATETIME, o.InvoiceDue)
		FROM
			[ETL].Staging_KNet_Invoice i inner join
			[ETL].Staging_OpenDetailAR o on i.InvoiceNumber = o.InvoiceNumber inner join
			#NewInvoices n on i.InvoiceNumber = n.InvoiceNumber  

	--Create Detail Records
	INSERT INTO [Invoice].[InvoiceItems] (InvoiceId, ItemNumber, ItemPrice, LineNumber, CatchWeightCode, ClassCode, ExtCatchWeight, ExtSalesNet,QuantityOrdered, QuantityShipped, CreatedUtc, ModifiedUtc)
		SELECT
			inv.Id,
			LTRIM(RTRIM(i.ItemNumber)),
			CONVERT(DECIMAL, i.ItemPrice),
			LTRIM(RTRIM(i.LineNumber)),
			CASE WHEN i.CatchWeightCode = 'Y' THEN 1 ELSE 0 END,
			SUBSTRING(LTRIM(RTRIM(i.ClassCode)), 1,2),
			CONVERT(DECIMAL, i.ExtCatchWeight),
			CONVERT(DECIMAL, i.ExtSalesNet),
			CONVERT(INTEGER, i.QuantityOrdered),
			CONVERT(INTEGER, i.QuantityShipped),
			GETUTCDATE(),
			GETUTCDATE()
		FROM
			[ETL].Staging_KNet_Invoice i inner join
			[Invoice].Invoices inv on i.InvoiceNumber = inv.InvoiceNumber inner join
			#NewInvoices n on i.InvoiceNumber = n.InvoiceNumber 

	/*************************************************************************
	/
	/	Mark paid invoices
	/
	**************************************************************************/
	UPDATE [Invoice].[Invoices] SET [Status] = 1 WHERE InvoiceNumber in (SELECT InvoiceNumber FROM [ETL].[Staging_PaidDetail])

	/*************************************************************************
	/
	/	Create new invoice records from the paid AR file. The only time this should 
	/	have anything is on the initial load, when the detail file is generated for 
	/	past invoices
	/
	**************************************************************************/
	INSERT INTO #OldInvoices (InvoiceNumber)
		SELECT 
			DISTINCT o.InvoiceNumber 
		FROM
			[ETL].Staging_KNet_Invoice i inner join
			[ETL].Staging_PaidDetail o on i.InvoiceNumber = o.InvoiceNumber
		WHERE
			o.InvoiceNumber NOT IN (SELECT InvoiceNumber FROM [Invoice].[Invoices])

	INSERT INTO [Invoice].[Invoices] (InvoiceNumber, OrderDate, InvoiceDate, CustomerNumber, CreatedUtc, ModifiedUtc, BranchId, [Type], Amount, [Status], DueDate)
		SELECT
			DISTINCT
			o.InvoiceNumber,
			CONVERT(DATETIME, i.OrderDate),
			CONVERT(DATETIME, i.ShipDate),
			o.Customer,
			GETUTCDATE(),
			GETUTCDATE(),
			o.Company,
			CASE 
				WHEN o.InvoiceNumber LIKE '%-C-%' THEN 1 
				WHEN o.InvoiceNumber LIKE '%-A-%' THEN 2
				ELSE 0
			END,
			CONVERT(decimal(18,2), o.InvoiceAmount),
			1, --Paid
			CONVERT(DATETIME, o.InvoiceDue)
		FROM
			[ETL].Staging_KNet_Invoice i inner join
			[ETL].Staging_PaidDetail o on i.InvoiceNumber = o.InvoiceNumber inner join
			#OldInvoices n on i.InvoiceNumber = n.InvoiceNumber  

	--Create Detail Records
	INSERT INTO [Invoice].[InvoiceItems] (InvoiceId, ItemNumber, ItemPrice, LineNumber, CatchWeightCode, ClassCode, ExtCatchWeight, ExtSalesNet,QuantityOrdered, QuantityShipped, CreatedUtc, ModifiedUtc)
		SELECT
			inv.Id,
			LTRIM(RTRIM(i.ItemNumber)),
			CONVERT(DECIMAL, i.ItemPrice),
			LTRIM(RTRIM(i.LineNumber)),
			CASE WHEN i.CatchWeightCode = 'Y' THEN 1 ELSE 0 END,
			SUBSTRING(LTRIM(RTRIM(i.ClassCode)), 1,2),
			CONVERT(DECIMAL, i.ExtCatchWeight),
			CONVERT(DECIMAL, i.ExtSalesNet),
			CONVERT(INTEGER, i.QuantityOrdered),
			CONVERT(INTEGER, i.QuantityShipped),
			GETUTCDATE(),
			GETUTCDATE()
		FROM
			[ETL].Staging_KNet_Invoice i inner join
			[Invoice].Invoices inv on i.InvoiceNumber = inv.InvoiceNumber inner join
			#OldInvoices n on i.InvoiceNumber = n.InvoiceNumber 

	--Cleanup
	DROP TABLE #NewInvoices
	DROP TABLE #OldInvoices
END

GO
/****** Object:  StoredProcedure [ETL].[ProcessWorksheetList]    Script Date: 10/27/2016 1:05:24 PM ******/
SET ANSI_NULLS OFF
GO
SET QUOTED_IDENTIFIER OFF
GO

-- =============================================
-- Author:		Joshua P. Tirey
-- Create date: 3/29/2015
-- Description:	Creates Entree list for Customer History (worksheet) items
-- =============================================
CREATE PROCEDURE [ETL].[ProcessWorksheetList]
AS
BEGIN

DECLARE @customerId varchar(15)
DECLARE @branchID varchar(50)


DECLARE @CurrentBidNumber varchar(15)
DECLARE @CurrentDivision varchar(15)
DECLARE @countT int
DECLARE @existingListId bigint

DECLARE @AddedItems TABLE
(
	ItemNumber varchar(10),
	Each bit
)
DECLARE @DeletedItems TABLE
(
	ItemNumber varchar(10),
	Each bit
)

--DECLARE Cursor for all contracts
DECLARE worksheet_Cursor CURSOR FAST_FORWARD FOR
	SELECT 
		[CustomerNumber],
		[DivisionNumber]
	FROM 
		[BEK_Commerce_AppData].[ETL].[Staging_WorksheetItems]
	GROUP BY
		[CustomerNumber],
		[DivisionNumber]


OPEN worksheet_Cursor
FETCH NEXT FROM worksheet_Cursor INTO @customerId, @branchID

WHILE @@FETCH_STATUS = 0
BEGIN
	--Find existing worksheet list for the customer
	SELECT @existingListId = Id from [BEK_Commerce_AppData].[List].Lists WHERE Type = 5 AND CustomerId = LTRIM(RTRIM(@customerId)) AND BranchId = LTRIM(RTRIM(@branchID))
	
	IF @existingListId IS NULL
		BEGIN
			--List doesn't exist -- Create list
			INSERT INTO [BEK_Commerce_AppData].[List].[Lists]
				([DisplayName]
				,[Type]
				,[CustomerId]
				,[BranchId]
				,[ReadOnly]
				,[CreatedUtc]
				,[ModifiedUtc])
			VALUES
				('History'
				,5
				,LTRIM(RTRIM(@customerId))
				,LTRIM(RTRIM(@branchID))
				,1
				,GETUTCDATE()
				,GETUTCDATE())

			SET @existingListId = SCOPE_IDENTITY();

			--Insert items into the new list
			INSERT INTO [BEK_Commerce_AppData].[List].[ListItems]
					   ([ItemNumber]
					   ,[Par]
					   ,[CreatedUtc]
					   ,[ParentList_Id]
					   ,[ModifiedUtc]
					   ,[Position]
					   ,[Each]
					   ,[CatalogId])
			SELECT 
				LTRIM(RTRIM(ItemNumber)),
				0.00,
				GETUTCDATE(),
				@existingListId,
				GETUTCDATE(),
				ROW_NUMBER() over (Order By ItemNumber),
				CASE WHEN BrokenCaseCode = 'Y' THEN 1 ELSE 0 END,
				@branchID
			FROM 
				[BEK_Commerce_AppData].[ETL].[Staging_WorksheetItems]
			WHERE
				[CustomerNumber] = @customerId 
				AND [DivisionNumber] = @branchID

		END
	ELSE
		BEGIN
			--List already exist. Update with new or deleted items
			
			
			--Find new items to be added
			INSERT INTO @AddedItems (ItemNumber, Each)
			SELECT 
				LTRIM(RTRIM(w.ItemNumber)), 
				CASE WHEN w.BrokenCaseCode = 'Y' THEN 1 ELSE 0 END
			FROM
				[BEK_Commerce_AppData].[ETL].[Staging_WorksheetItems] w
			WHERE
				w.CustomerNumber = @customerId AND
				w.DivisionNumber = @branchID AND
				NOT EXISTS(SELECT 'x' FROM [BEK_Commerce_AppData].[List].ListItems li 
							WHERE li.ItemNumber = LTRIM(RTRIM(w.ItemNumber)) AND li.Each = CASE WHEN w.BrokenCaseCode = 'Y' THEN 1 ELSE 0 END AND li.ParentList_Id = @existingListId)
						

			--Find items being deleted
			INSERT INTO @DeletedItems (ItemNumber, Each)
			SELECT
				l.ItemNumber,
				l.Each
			FROM
				[BEK_Commerce_AppData].[List].[ListItems] l
			WHERE
				l.ParentList_Id = @existingListId AND
				NOT EXISTS(SELECT 
						'x'
					FROM
						[BEK_Commerce_AppData].[ETL].[Staging_WorksheetItems] w
					WHERE
						LTRIM(RTRIM(w.ItemNumber)) = l.ItemNumber AND
						CASE WHEN w.BrokenCaseCode = 'Y' THEN 1 ELSE 0 END = l.Each AND
						w.CustomerNumber = @customerId AND w.DivisionNumber = @branchID)
			--New items to add?
			IF EXISTS(SELECT 'x' FROM @AddedItems)
				BEGIN
					--Insert items into the list
					INSERT INTO [BEK_Commerce_AppData].[List].[ListItems]
							   ([ItemNumber]
							   ,[Par]
							   ,[CreatedUtc]
							   ,[ParentList_Id]
							   ,[ModifiedUtc]
							   ,[Position]
							   ,[Each]
							   ,[CatalogId])
					SELECT
						LTRIM(RTRIM(ItemNumber)),
						0.00,
						GETUTCDATE(),
						@existingListId,
						GETUTCDATE(),
						0,
						Each,
						@branchID
					FROM
						@AddedItems

				END

			--Items to delete
			IF EXISTS(SELECT 'x' FROM @DeletedItems)
				BEGIN
					--DELETE Item
					DELETE [BEK_Commerce_AppData].[List].[ListItems]
					FROM [BEK_Commerce_AppData].[List].[ListItems] li INNER JOIN
						@DeletedItems d on li.ItemNumber = d.ItemNumber AND li.Each = d.Each AND ParentList_Id = @existingListId
										
				END		
			

			--update all list position numbers 
			-- also update the catalog id to make sure that they get set
			UPDATE List.ListItems 
			SET Position = p.Positions
				, CatalogId = @branchID
			FROM List.Listitems 
				INNER JOIN
					(
						SELECT
							ItemNumber 'p_ItemNumber', 
							ParentList_Id 'p_ListId',
							RANK() OVER (ORDER BY ItemNumber) Positions
						FROM
							 List.listItems
						WHERE
							 ParentList_Id = @existingListId
					) as p
				ON ItemNumber = p.p_ItemNumber
				AND ParentList_Id = p.p_ListId
		END

	SET @existingListId = null
	DELETE FROM @AddedItems
	DELETE FROM @DeletedItems
	
	FETCH NEXT FROM worksheet_Cursor INTO @customerId, @branchID
END

CLOSE worksheet_Cursor
DEALLOCATE worksheet_Cursor
END

GO
/****** Object:  StoredProcedure [ETL].[PurgeInternalUserAccess]    Script Date: 10/27/2016 1:05:24 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [ETL].[PurgeInternalUserAccess]
AS
TRUNCATE TABLE [Customers].[InternalUserAccess]
RETURN 0


GO
/****** Object:  StoredProcedure [ETL].[PurgeOrderHistory_AppData]    Script Date: 10/27/2016 1:05:24 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE PROCEDURE [ETL].[PurgeOrderHistory_AppData] AS 

/*******************************************************************
* PROCEDURE: PurgeOrderHistory
* PURPOSE: Purge 6 months of order history.  Uses transactions blocks
* of 100,000 so it doesn't fill up transaction log.
* NOTES: {special set up or requirements, etc.}
* CREATED:	Jason McMillan 2015-11-25
* MODIFIED 
* DATE		AUTHOR			DESCRIPTION
*-------------------------------------------------------------------
* {date}	{developer}	{brief modification description}
*******************************************************************/

SET NOCOUNT ON;

/*****DELETE APP DATA ORDER HISTORY TRANSACTIONS*****/

--ORDER HISTORY DETAIL
DECLARE @Counter_d int;
SET @Counter_d = (
		SELECT 
			COUNT([Id])
		FROM 
			Orders.OrderHistoryDetail
		WHERE
			OrderHistoryHeader_Id IN (SELECT [Id] FROM Orders.OrderHistoryHeader WHERE CONVERT(date, CreatedUtc) <= DATEADD(mm, -6, GETDATE()))
	);

WHILE @Counter_d > 0
	BEGIN
		BEGIN TRANSACTION
		DELETE TOP(100000) FROM 
			Orders.OrderHistoryDetail
		WHERE
			OrderHistoryHeader_Id IN (SELECT [Id] FROM Orders.OrderHistoryHeader WHERE CONVERT(date, CreatedUtc) <= DATEADD(mm, -6, GETDATE()));
		IF @@ERROR = 0
			COMMIT TRANSACTION
		ELSE
			ROLLBACK TRANSACTION
		
		SET @Counter_d = (
			SELECT 
				COUNT([Id])
			FROM 
				Orders.OrderHistoryDetail
			WHERE
				OrderHistoryHeader_Id IN (SELECT [Id] FROM Orders.OrderHistoryHeader WHERE CONVERT(date, CreatedUtc) <= DATEADD(mm, -6, GETDATE()))
		);
	END


--ORDER HISTORY HEADER
DECLARE @Counter_h int;
SET @Counter_h = (
		SELECT 
			COUNT([Id])
		FROM 
			Orders.OrderHistoryHeader
		WHERE
			CONVERT(date, CreatedUtc) <= DATEADD(mm, -6, GETDATE())
	);

WHILE @Counter_h > 0
	BEGIN
		BEGIN TRANSACTION
		DELETE TOP(100000) FROM 
			Orders.OrderHistoryHeader
		WHERE
			CONVERT(date, CreatedUtc) <= DATEADD(mm, -6, GETDATE())
		IF @@ERROR = 0
			COMMIT TRANSACTION
		ELSE
			ROLLBACK TRANSACTION
		
		SET @Counter_h = (
			SELECT 
				COUNT([Id])
			FROM 
				Orders.OrderHistoryHeader
			WHERE
				CONVERT(date, CreatedUtc) <= DATEADD(mm, -6, GETDATE())
		);
	END


GO
/****** Object:  StoredProcedure [ETL].[ReadAverageItemUsage]    Script Date: 10/27/2016 1:05:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [ETL].[ReadAverageItemUsage]	
	@NumDays int
AS

--NEED TO ADD SUMMARY COMMENTS HERE

SET NOCOUNT ON;

SELECT
	oh.BranchId
	, oh.CustomerNumber
	, od.ItemNumber
	, od.unitOfMeasure
	, AVG(od.ShippedQuantity) 'AverageUse'
FROM 
	Orders.OrderHistoryHeader oh
		INNER JOIN Orders.OrderHistoryDetail od ON od.OrderHistoryHeader_Id = oh.Id
WHERE 
	oh.CreatedUtc > DATEADD(DD, (@NumDays * -1), GETDATE())
GROUP BY 
	oh.BranchId
	, oh.CustomerNumber
	, od.ItemNumber
	, od.unitOfMeasure

GO
/****** Object:  StoredProcedure [ETL].[ReadBranches]    Script Date: 10/27/2016 1:05:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [ETL].[ReadBranches]
	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT * FROM [ETL].Staging_Branch WHERE LocationTypeId=3
END




GO
/****** Object:  StoredProcedure [ETL].[ReadBrandControlLabels]    Script Date: 10/27/2016 1:05:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [ETL].[ReadBrandControlLabels]
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT
		b.ControlLabel,
		b.ExtendedDescription
	FROM
		ETL.Staging_BRandControlLabels b
END



GO
/****** Object:  StoredProcedure [ETL].[ReadContractItems]    Script Date: 10/27/2016 1:05:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [ETL].[ReadContractItems]
	@CustomerNumber varchar(10)
	, @DivisionName char(3)
	, @ContractNumber varchar(10)
 AS

 SET NOCOUNT ON

/*******************************************************************
* PROCEDURE: usp_ECOM_SelectDistinctCustomerContracts
* PURPOSE: Select distinct customer contracts by division
* NOTES: {special set up or requirements, etc.}
* CREATED:	Jason McMillan 9/30/14
* MODIFIED 
* DATE		AUTHOR			DESCRIPTION
*-------------------------------------------------------------------
* {date}	{developer}	{brief modification description}
*******************************************************************/

SELECT
	c.CustomerNumber
	, c.CustomerName
	, c.ContractOnly
	, LTRIM(RTRIM(c.[Contract])) 'Contract'
	, LTRIM(RTRIM(cb.BidNumber)) 'BidNumber'
	, bh.CompanyNumber
	, bh.DivisionNumber 'DivisionName'
	, bh.DepartmentNumber
	, bh.BidDescription
	, LTRIM(RTRIM(bd.ItemNumber)) 'ItemNumber'
	, bd.BidLineNumber
	, bd.CategoryNumber
	, bd.CategoryDescription
	, CASE
		WHEN ForceEachOrCaseOnly = 'B' THEN 'Y'
		WHEN ForceEachOrCaseOnly = 'C' THEN 'N'
		ELSE 'N'
	END AS 'BrokenCaseCode' --change name and values to match worksheet item query
FROM
	ETL.Staging_Customer c 
	INNER JOIN ETL.Staging_CustomerBid cb on c.CustomerNumber = cb.CustomerNumber 
		AND c.DIV = cb.DivisionNumber
		AND c.CO = cb.CompanyNumber
		AND c.DEPT = DepartmentNumber
	INNER JOIN ETL.Staging_BidContractHeader bh ON cb.BidNumber = bh.BidNumber 
		AND cb.DivisionNumber = bh.DivisionNumber
		AND cb.CompanyNumber = bh.CompanyNumber
		AND cb.DepartmentNumber = bh.DepartmentNumber
	INNER JOIN ETL.Staging_BidContractDetail bd ON bh.BidNumber = bd.BidNumber 
		AND cb.DivisionNumber = bh.DivisionNumber
		AND bh.CompanyNumber = bd.CompanyNumber
		AND bh.DepartmentNumber = bd.DepartmentNumber
WHERE
	LOWER(LTRIM(RTRIM(bh.BidNumber))) = LOWER(@ContractNumber)
	AND LTRIM(RTRIM(c.CustomerNumber)) = @CustomerNumber
	AND LOWER(LTRIM(RTRIM(cb.DivisionNumber))) = LOWER(@DivisionName)
ORDER BY
	bd.BidLineNumber ASC


/*
EXEC ETL.usp_ECOM_SelectContractItems '415101', 'FAM', 'D415101'
*/

GO
/****** Object:  StoredProcedure [ETL].[ReadCSUsers]    Script Date: 10/27/2016 1:05:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [ETL].[ReadCSUsers]
AS

/*******************************************************************
* PROCEDURE: usp_ECOM_SelectCSUsers
* PURPOSE: Select distinct commerce server users and email addresses
* NOTES: {special set up or requirements, etc.}
* CREATED:	Jason McMillan 2014-10-10
* MODIFIED 
* DATE		AUTHOR			DESCRIPTION
*-------------------------------------------------------------------
* {date}	{developer}	{brief modification description}
*******************************************************************/

SET NOCOUNT ON

SELECT
	DISTINCT
	u_user_id
FROM
	[BEK_Commerce_profiles]..[UserObject]


GO
/****** Object:  StoredProcedure [ETL].[ReadCustomers]    Script Date: 10/27/2016 1:05:25 PM ******/
SET ANSI_NULLS OFF
GO
SET QUOTED_IDENTIFIER OFF
GO




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

GO
/****** Object:  StoredProcedure [ETL].[ReadDsrImage]    Script Date: 10/27/2016 1:05:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [ETL].[ReadDsrImage] AS

/*******************************************************************
* PROCEDURE: ReadDsrImage
* PURPOSE: 
* NOTES: {special set up or requirements, etc.}
* CREATED:	Jason McMillan 1/28/2015
* MODIFIED 
* DATE		AUTHOR			DESCRIPTION
*-------------------------------------------------------------------
* {date}	{developer}	{brief modification description}
*******************************************************************/

SET NOCOUNT ON;

SELECT
	LTRIM(RTRIM(LOWER(d.EmailAddress))) 'EmailAddress'
	, e.EMPLOYEE_PHOTO 'EmployeePhoto'
FROM
	ETL.Staging_EmployeeInfo e
	INNER JOIN ETL.Staging_Dsr d ON LTRIM(RTRIM(LOWER(e.EMAIL_ADDR))) = LTRIM(RTRIM(LOWER(d.EmailAddress)))


GO
/****** Object:  StoredProcedure [ETL].[ReadDsrInfo]    Script Date: 10/27/2016 1:05:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [ETL].[ReadDsrInfo] AS

/*******************************************************************
* PROCEDURE: ReadDsrInfo
* PURPOSE: 
* NOTES: {special set up or requirements, etc.}
* CREATED:	Jason McMillan 1/28/2015
* MODIFIED 
* DATE		AUTHOR			DESCRIPTION
*-------------------------------------------------------------------
* {date}	{developer}	{brief modification description}
*******************************************************************/

SET NOCOUNT ON;

SELECT
	RIGHT(LTRIM(RTRIM(d.DsrNumber)), 3) 'DsrNumber'
	, LTRIM(RTRIM(LOWER(d.EmailAddress))) 'EmailAddress'
	, CASE LEFT(d.DsrNumber, 2)
		WHEN 'AM' THEN 'FAM'
		WHEN 'AQ' THEN 'FAQ'
		WHEN 'AR' THEN 'FAR'
		WHEN 'DF' THEN 'FDF'
		WHEN 'HS' THEN 'FHS'
		WHEN 'LR' THEN 'FLR'
		WHEN 'OK' THEN 'FOK'
		WHEN 'SA' THEN 'FSA'
		WHEN 'ZN' THEN 'ZZZZZZZ'
	END 'BranchId'
	, CONCAT(SUBSTRING(Name, CHARINDEX(',',Name,0) + 1, LEN(Name) - CHARINDEX(',',Name,0)), ' ', SUBSTRING(Name, 0, CHARINDEX(',',Name,0))) 'Name'
	, REPLACE(REPLACE(LTRIM(RTRIM(e.PHONE)),'/',''),'-','') 'Phone'
	, CONCAT('{baseUrl}/userimages/',LTRIM(RTRIM(LOWER(d.EmailAddress)))) 'ImageUrl'
FROM
	ETL.Staging_EmployeeInfo e
	INNER JOIN ETL.Staging_Dsr d ON LTRIM(RTRIM(LOWER(e.EMAIL_ADDR))) = LTRIM(RTRIM(LOWER(d.EmailAddress)))

GO
/****** Object:  StoredProcedure [ETL].[ReadFullItemData]    Script Date: 10/27/2016 1:05:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [ETL].[ReadFullItemData]
	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	SELECT 
		BranchId 
		, ItemId 
		, ETL.initcap(Name) as Name
		, ETL.initcap(Description) as Description
		, i.Brand
		, Pack
		, Size
		, UPC
		, MfrNumber
		, MfrName
		, (SELECT b.ExtendedDescription FROM ETL.Staging_Brands b WHERE b.Brand = i.Brand) As BrandDescription
		, MaxSmrt
		, Cases
		, Package
		, PreferredItemCode
		, ItemType
		, Status1
		, Status2
		, ICSEOnly
		, SpecialOrderItem
		, Vendor1
		, Vendor2
		, Class
		, CatMgr
		, HowPrice
		, Buyer
		, Kosher
		, c.CategoryId
		, ReplacementItem
		, ReplacedItem
		, CNDoc
		, [Cube]
		, ETL.initcap(c.CategoryName) as CategoryName
		, (SELECT CategoryId from ETL.Staging_Category WHERE CategoryId = SUBSTRING(c.CategoryId, 1, 2) + '000') as ParentCategoryId
		, (SELECT ETL.initcap(CategoryName) from ETL.Staging_Category WHERE CategoryId = SUBSTRING(c.CategoryId, 1, 2) + '000') as ParentCategoryName
		, ps.BrandOwner
		, ps.CountryOfOrigin
		, ps.GrossWeight
		, ps.HandlingInstruction
		, ps.Height
		, ps.Ingredients
		, ps.Length
		, ps.MarketingMessage
		, ps.MoreInformation
		, ps.ServingSize
		, ps.ServingSizeUOM
		, ps.ServingsPerPack
		, ps.ServingSuggestion
		, ps.Shelf
		, ps.StorageTemp
		, ps.UnitMeasure
		, ps.UnitsPerCase
		, ps.Volume
		, ps.Width
		, CASE i.NonStock
			WHEN 'C' THEN 'Y'
			WHEN 'N' THEN 'Y'
			WHEN 'Y' THEN 'N'
			WHEN 'S' THEN 'N'
			WHEN ' ' THEN 'N'
		ELSE 'Y' END AS 'NonStock'
		, i.FDAProductFlag
		, i.TempZone
		, i.FPNetWt
		, i.GrossWeight 'GrossWt'	 
	FROM  
		ETL.Staging_ItemData i inner join 
		ETL.Staging_Category c on i.CategoryId = c.CategoryId left outer join
		ETL.Staging_FSE_ProductSpec ps on i.UPC = ps.Gtin
	WHERE 
		  i.ItemId NOT LIKE '999%'  AND SpecialOrderItem <>'Y'
END

GO
/****** Object:  StoredProcedure [ETL].[ReadInvoices]    Script Date: 10/27/2016 1:05:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [ETL].[ReadInvoices]

AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
		SELECT
	CustomerNumber,
	InvoiceNumber,
	ShipDate, 
	OrderDate,
	ItemNumber,
	QuantityOrdered,
	QuantityShipped,
	CatchWeightCode,
	ExtCatchWeight,
	ItemPrice,
	ExtSalesNet,
	ClassCode,
	LineNumber
FROM [ETL].[Staging_KNet_Invoice]
ORDER BY InvoiceNumber
END




GO
/****** Object:  StoredProcedure [ETL].[ReadItemGS1Data]    Script Date: 10/27/2016 1:05:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [ETL].[ReadItemGS1Data]
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT
		n.DailyValue,
		n.Gtin,
		n.MeasurementValue,
		n.MeasurmentTypeId,
		n.NutrientTypeCode,
		n.NutrientTypeDesc
	FROM
		ETL.Staging_FSE_ProductNutrition n
	WHERE
		n.DailyValue IS NOT NULL

	SELECT
		d.Gtin,
		d.DietType,
		d.Value
	FROM
		ETL.Staging_FSE_ProductDiet d
	WHERE
		d.Value IS NOT NULL

	SELECT
		a.Gtin,
		a.AllergenTypeCode,
		a.AllergenTypeDesc,
		a.LevelOfContainment		
	FROM
		ETL.Staging_FSE_ProductAllergens a
	WHERE
		a.AllergenTypeDesc IS NOT NULL
END



GO
/****** Object:  StoredProcedure [ETL].[ReadItemsByBranch]    Script Date: 10/27/2016 1:05:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [ETL].[ReadItemsByBranch]
	@branchId nvarchar(3)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	SELECT DISTINCT 
		i.[ItemId] 
		,ETL.initcap([Name]) as Name 
		,ETL.initcap([Description]) as Description 
		,ETL.initcap([Brand]) as Brand 
		,[Pack] 
		,[Size] 
		,[UPC] 
		,[MfrNumber] 
		,ETL.initcap([MfrName]) as MfrName 
		,i.CategoryId 
	FROM [ETL].[Staging_ItemData] i inner join 
		ETL.Staging_Category c on i.CategoryId = c.CategoryId 
	WHERE 
		i.BranchId = @branchId AND ItemId NOT LIKE '999%' AND SpecialOrderItem <>'Y'
	Order by i.[ItemId]
END




GO
/****** Object:  StoredProcedure [ETL].[ReadParentCategories]    Script Date: 10/27/2016 1:05:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [ETL].[ReadParentCategories]
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	SELECT 
		CategoryId, 
		[ETL].initcap(CategoryName) as CategoryName, 
		PPICode 
	FROM 
		[ETL].Staging_Category 
	WHERE 
		CategoryId like '%000'
END





GO
/****** Object:  StoredProcedure [ETL].[ReadProprietaryItems]    Script Date: 10/27/2016 1:05:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [ETL].[ReadProprietaryItems]
	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	SELECT
		DISTINCT
		c.CustomerNumber,
		i.ItemNumber
	FROM
		ETL.Staging_ProprietaryItem i INNER JOIN
		ETL.Staging_ProprietaryCustomer c on i.ProprietaryNumber = c.ProprietaryNumber
	Order By
		i.ItemNumber
END


GO
/****** Object:  StoredProcedure [ETL].[ReadSubCategories]    Script Date: 10/27/2016 1:05:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [ETL].[ReadSubCategories]
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	SELECT 
		SUBSTRING(CategoryId, 1, 2) + '000' AS ParentCategoryId, 
		CategoryId, 
		[ETL].initcap(CategoryName) as CategoryName, 
		PPICode 
	FROM 
		[ETL].Staging_Category 
	WHERE 
		CategoryId not like '%000'
END





GO
/****** Object:  StoredProcedure [ETL].[ReadUNFICategories]    Script Date: 10/27/2016 1:05:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE PROCEDURE [ETL].[ReadUNFICategories]
	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT DISTINCT SUBSTRING(TCSCode, 1, 3) + '00' AS CategoryId,[ETL].initcap(Category) as CategoryName, [Type] AS Department FROM [ETL].Staging_UNFIProducts
END

GO
/****** Object:  StoredProcedure [ETL].[ReadUNFIDistinctWarehouses]    Script Date: 10/27/2016 1:05:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE PROCEDURE [ETL].[ReadUNFIDistinctWarehouses]
	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT DISTINCT WarehouseNumber FROM [ETL].Staging_UNFIProducts
END

GO
/****** Object:  StoredProcedure [ETL].[ReadUNFIProducts]    Script Date: 10/27/2016 1:05:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [ETL].[ReadUNFIProducts]
	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT * FROM [ETL].Staging_UNFIProducts
END

GO
/****** Object:  StoredProcedure [ETL].[ReadUNFISubCategories]    Script Date: 10/27/2016 1:05:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE PROCEDURE [ETL].[ReadUNFISubCategories]
	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT DISTINCT SUBSTRING(TCSCode, 1, 3) + '00' AS ParentCategoryId,TCSCode as CategoryId,[ETL].initcap(SubGroup) as CategoryName, [Type] as Department FROM [ETL].Staging_UNFIProducts
END

GO
/****** Object:  StoredProcedure [ETL].[ReadUNFItemsByWarehouse]    Script Date: 10/27/2016 1:05:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [ETL].[ReadUNFItemsByWarehouse]
	@warehouse nvarchar(3)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	SELECT DISTINCT 
		ProductNumber,
		[Description],
		Category,
		Subgroup
	FROM [ETL].[Staging_UNFIProducts]
	WHERE 
		WarehouseNumber = @warehouse
	Order by ProductNumber
END

GO
/****** Object:  StoredProcedure [ETL].[ReadWorksheetItems]    Script Date: 10/27/2016 1:05:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [ETL].[ReadWorksheetItems]
	@CustomerNumber varchar(10)
	, @DivisionName char(3)
 AS

 SET NOCOUNT ON

/*******************************************************************
* PROCEDURE: usp_ECOM_SelectDistinctCustomerContracts
* PURPOSE: Select distinct customer contracts by division
* NOTES: {special set up or requirements, etc.}
* CREATED:	Jason McMillan 9/30/14
* MODIFIED 
* DATE		AUTHOR			DESCRIPTION
*-------------------------------------------------------------------
* {date}	{developer}	{brief modification description}
*******************************************************************/

SELECT
	c.CustomerNumber
	, c.CustomerName
	, LTRIM(RTRIM(cw.ItemNumber)) as ItemNumber
	, cw.BrokenCaseCode
	, cw.ItemPrice
	, cw.QtyOrdered
	, cw.DateOfLastOrder
FROM
	ETL.Staging_Customer c 
	INNER JOIN ETL.Staging_WorksheetItems cw on c.CustomerNumber = cw.CustomerNumber 
		AND c.DIV = cw.DivisionNumber
		AND c.CO = cw.CompanyNumber
		AND c.DEPT = cw.DepartmentNumber
	INNER JOIN ETL.Staging_ItemData i ON LTRIM(RTRIM(cw.ItemNumber)) = LTRIM(RTRIM(i.ItemId)) 
		AND c.DIV = i.BranchId
WHERE
	LTRIM(RTRIM(c.CustomerNumber)) = @CustomerNumber
	AND LOWER(LTRIM(RTRIM(cw.DivisionNumber))) = LOWER(@DivisionName)
	AND i.ItemId NOT LIKE '999%' AND i.SpecialOrderItem <>'Y'
ORDER BY
	cw.ItemNumber ASC


/*
EXEC ETL.usp_ECOM_SelectWorksheetItems '415101', 'FAM'
*/

GO
/****** Object:  StoredProcedure [Orders].[usp_GetLastFiveOrdersForItem]    Script Date: 10/27/2016 1:05:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [Orders].[usp_GetLastFiveOrdersForItem]
       @BranchId            CHAR(3),
       @CustomerNumber      CHAR(6),
       @ItemNumber          CHAR(6)
AS
       SELECT 
              TOP 5
              *
       FROM
              ORDERS.ORDERHISTORYHEADER H
       INNER JOIN
              ORDERS.ORDERHISTORYDETAIL D
       ON
              (D.ORDERHISTORYHEADER_ID = H.ID)
       WHERE
              H.BRANCHID = @BranchId
       AND
              H.CUSTOMERNUMBER = @CustomerNumber
       AND
              D.ITEMNUMBER = @ItemNumber
       ORDER BY
              CAST(H.DELIVERYDATE AS DATETIME) DESC

GO
/****** Object:  StoredProcedure [Orders].[usp_GetNextControlNumber]    Script Date: 10/27/2016 1:05:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [Orders].[usp_GetNextControlNumber]
	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	declare @maxVal int;
	declare @currVal int;
	declare @beginVal int;

    BEGIN TRANSACTION;
    -- Increment the counter 
    UPDATE IdentityCounter SET CurrentId = CurrentId + 1 WHERE CounterName='ControlNumber'

    -- Let's get the current value
    SELECT @maxVal = EndId, @currVal=CurrentId,@beginVal=StartId FROM [Orders].[IdentityCounter] WHERE CounterName='ControlNumber'
	IF (@currVal > @maxVal)
		BEGIN
			SET @currVal = @beginVal
			UPDATE IdentityCounter SET CurrentId=@currVal WHERE CounterName='ControlNumber'
		END
	
    COMMIT;
	return @currVal
END


GO
/****** Object:  UserDefinedFunction [ETL].[initcap]    Script Date: 10/27/2016 1:05:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE function [ETL].[initcap] (@text varchar(4000))
returns varchar(4000)
as

begin
	declare 	@counter int, 
		@length int,
		@char char(1),
		@textnew varchar(4000)
	if(@text = '')
	begin
		return @text
	end

	set @text		= rtrim(@text)
	set @text		= lower(@text)
	set @length 	= len(@text)
	set @counter 	= 1

	set @text = upper(left(@text, 1) ) + right(@text, @length - 1) 

	while @counter <> @length --+ 1
	begin
		select @char = substring(@text, @counter, 1)

		IF @char = space(1)  or @char =  '_' or @char = ','  or @char = '.' or @char = '\'
 or @char = '/' or @char = '(' or @char = ')'
		begin
			set @textnew = left(@text, @counter)  + upper(substring(@text, 
@counter+1, 1)) + right(@text, (@length - @counter) - 1)
			set @text	 = @textnew
		end

		set @counter = @counter + 1
	end

	return @text
end



GO
/****** Object:  Table [ETL].[Staging_ItemData]    Script Date: 10/27/2016 1:05:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [ETL].[Staging_ItemData](
	[BranchId] [char](3) NOT NULL,
	[ItemId] [char](6) NOT NULL,
	[Name] [varchar](30) NULL,
	[Description] [varchar](30) NULL,
	[Brand] [varchar](8) NULL,
	[Pack] [char](4) NULL,
	[Size] [varchar](8) NULL,
	[UPC] [varchar](14) NULL,
	[MfrNumber] [varchar](10) NULL,
	[MfrName] [varchar](30) NULL,
	[Cases] [int] NULL,
	[Package] [int] NULL,
	[PreferredItemCode] [char](1) NULL,
	[CategoryId] [char](6) NULL,
	[ItemType] [char](1) NULL,
	[Status1] [char](1) NULL,
	[Status2] [char](1) NULL,
	[ICSEOnly] [char](1) NULL,
	[SpecialOrderItem] [char](1) NULL,
	[Vendor1] [char](6) NULL,
	[Vendor2] [char](6) NULL,
	[Class] [char](2) NULL,
	[CatMgr] [char](2) NULL,
	[HowPrice] [char](1) NULL,
	[Buyer] [char](2) NULL,
	[Kosher] [char](1) NULL,
	[PVTLbl] [char](1) NULL,
	[MaxSmrt] [char](2) NULL,
	[OrderTiHi] [int] NULL,
	[TiHi] [int] NULL,
	[DateDiscontinued] [int] NULL,
	[Dtelstal] [int] NULL,
	[DTELstPO] [int] NULL,
	[GrossWeight] [int] NULL,
	[NetWeight] [int] NULL,
	[ShelfLife] [int] NULL,
	[DateSensitiveType] [char](1) NULL,
	[Country] [char](10) NULL,
	[Length] [int] NULL,
	[Width] [int] NULL,
	[Height] [int] NULL,
	[Cube] [int] NULL,
	[MinTemp] [int] NULL,
	[MaxTemp] [int] NULL,
	[GDSNSync] [char](1) NULL,
	[GuaranteedDays] [int] NULL,
	[MasterPack] [int] NULL,
	[ReplacementItem] [char](6) NULL,
	[ReplacedItem] [char](6) NULL,
	[TempZone] [char](1) NULL,
	[CNDoc] [char](1) NULL,
	[HACCP] [char](1) NULL,
	[HACCPDoce] [char](5) NULL,
	[FDAProductFlag] [char](1) NULL,
	[FPLength] [int] NULL,
	[FPWidth] [int] NULL,
	[FPHeight] [int] NULL,
	[FPGrossWt] [int] NULL,
	[FPNetWt] [int] NULL,
	[FPCube] [int] NULL,
	[NonStock] [char](1) NULL
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  UserDefinedFunction [ETL].[ConcatBranchesForItem]    Script Date: 10/27/2016 1:05:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE FUNCTION [ETL].[ConcatBranchesForItem] (
	@vendorId char(6),
	@itemId char(6)
)

RETURNS VARCHAR(50)
WITH SCHEMABINDING
AS
BEGIN

	DECLARE @ListOfBranches VARCHAR(50)

	SELECT @ListOfBranches = COALESCE(@ListOfBranches + ',', '') + BranchId
	FROM ETL.Staging_ItemData
	WHERE Vendor1 = @vendorId
	AND ItemId = @itemId

	RETURN (@ListOfBranches)

END

GO
/****** Object:  Table [BranchSupport].[BranchSupports]    Script Date: 10/27/2016 1:05:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [BranchSupport].[BranchSupports](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[BranchName] [nvarchar](max) NULL,
	[BranchId] [nvarchar](max) NULL,
	[SupportPhoneNumber] [nvarchar](max) NULL,
	[TollFreeNumber] [nvarchar](max) NULL,
	[Email] [nvarchar](max) NULL,
	[CreatedUtc] [datetime] NOT NULL,
	[ModifiedUtc] [datetime] NOT NULL,
 CONSTRAINT [PK_BranchSupport.BranchSupports] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [BranchSupport].[DsrAliases]    Script Date: 10/27/2016 1:05:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [BranchSupport].[DsrAliases](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[UserId] [uniqueidentifier] NOT NULL,
	[UserName] [varchar](200) NOT NULL,
	[BranchId] [char](3) NOT NULL,
	[CreatedUtc] [datetime] NOT NULL,
	[ModifiedUtc] [datetime] NOT NULL,
	[DsrNumber] [char](6) NOT NULL,
 CONSTRAINT [PK_BranchSupport.DsrAliases] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [BranchSupport].[Dsrs]    Script Date: 10/27/2016 1:05:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [BranchSupport].[Dsrs](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[DsrNumber] [char](3) NULL,
	[EmailAddress] [varchar](200) NULL,
	[BranchId] [char](3) NULL,
	[Name] [nvarchar](50) NULL,
	[Phone] [nvarchar](50) NULL,
	[ImageUrl] [nvarchar](200) NULL,
	[CreatedUtc] [datetime] NOT NULL DEFAULT (getutcdate()),
	[ModifiedUtc] [datetime] NOT NULL DEFAULT (getutcdate()),
 CONSTRAINT [PK_BranchSupport.Dsrs] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [Configuration].[AppSettings]    Script Date: 10/27/2016 1:05:25 PM ******/
SET ANSI_NULLS OFF
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [Configuration].[AppSettings](
	[Key] [varchar](50) NOT NULL,
	[Value] [varchar](max) NOT NULL,
	[Comment] [varchar](max) NOT NULL,
	[Disabled] [bit] NOT NULL DEFAULT ((0)),
PRIMARY KEY CLUSTERED 
(
	[Key] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [Configuration].[ExportSettings]    Script Date: 10/27/2016 1:05:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [Configuration].[ExportSettings](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[UserId] [uniqueidentifier] NOT NULL,
	[Type] [int] NOT NULL,
	[ListType] [int] NULL,
	[Settings] [nvarchar](max) NULL,
	[CreatedUtc] [datetime] NOT NULL DEFAULT (getutcdate()),
	[ModifiedUtc] [datetime] NOT NULL DEFAULT (getutcdate()),
	[ExportFormat] [nvarchar](max) NULL,
 CONSTRAINT [PK_Configuration.ExportSettings] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [Configuration].[ExternalCatalogs]    Script Date: 10/27/2016 1:05:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [Configuration].[ExternalCatalogs](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[BekBranchId] [nvarchar](24) NULL,
	[ExternalBranchId] [nvarchar](24) NULL,
	[Type] [int] NOT NULL,
	[CreatedUtc] [datetime] NOT NULL DEFAULT (getutcdate()),
	[ModifiedUtc] [datetime] NOT NULL DEFAULT (getutcdate()),
 CONSTRAINT [PK_Configuration.ExternalCatalogs] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [Configuration].[MessageTemplates]    Script Date: 10/27/2016 1:05:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [Configuration].[MessageTemplates](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[TemplateKey] [nvarchar](50) NULL,
	[Subject] [nvarchar](max) NULL,
	[IsBodyHtml] [bit] NOT NULL,
	[Body] [nvarchar](max) NULL,
	[CreatedUtc] [datetime] NOT NULL DEFAULT (getutcdate()),
	[ModifiedUtc] [datetime] NOT NULL DEFAULT (getutcdate()),
	[Type] [int] NOT NULL DEFAULT ((0)),
 CONSTRAINT [PK_Configuration.EmailTemplates] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [Customers].[InternalUserAccess]    Script Date: 10/27/2016 1:05:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [Customers].[InternalUserAccess](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[BranchId] [char](3) NOT NULL,
	[CustomerNumber] [char](6) NULL,
	[UserId] [uniqueidentifier] NOT NULL,
	[CustomerId] [uniqueidentifier] NOT NULL,
	[RoleId] [varchar](70) NOT NULL,
	[EmailAddress] [varchar](200) NOT NULL,
	[CreatedUtc] [datetime] NOT NULL DEFAULT (getutcdate()),
	[ModifiedUtc] [datetime] NOT NULL DEFAULT (getutcdate()),
 CONSTRAINT [PK_Customers.InternalUserAccess] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [Customers].[ItemHistory]    Script Date: 10/27/2016 1:05:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [Customers].[ItemHistory](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[BranchId] [char](3) NULL,
	[CustomerNumber] [char](6) NULL,
	[ItemNumber] [char](6) NULL,
	[CreatedUtc] [datetime] NOT NULL DEFAULT (getutcdate()),
	[ModifiedUtc] [datetime] NOT NULL DEFAULT (getutcdate()),
	[UnitOfMeasure] [char](1) NULL,
	[AverageUse] [int] NOT NULL DEFAULT ((0)),
 CONSTRAINT [PK_Customers.ItemHistory] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[__MigrationHistory]    Script Date: 10/27/2016 1:05:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[__MigrationHistory](
	[MigrationId] [nvarchar](150) NOT NULL,
	[ContextKey] [nvarchar](300) NOT NULL,
	[Model] [varbinary](max) NOT NULL,
	[ProductVersion] [nvarchar](32) NOT NULL,
 CONSTRAINT [PK_dbo.__MigrationHistory] PRIMARY KEY CLUSTERED 
(
	[MigrationId] ASC,
	[ContextKey] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[AuditLog]    Script Date: 10/27/2016 1:05:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AuditLog](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[Type] [int] NOT NULL,
	[TypeDescription] [nvarchar](50) NULL,
	[Actor] [nvarchar](100) NULL,
	[Information] [nvarchar](max) NULL,
	[CreatedUtc] [datetime] NOT NULL DEFAULT (getutcdate()),
 CONSTRAINT [PK_Auditing.AuditRecords] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Log]    Script Date: 10/27/2016 1:05:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Log](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Date] [datetime] NOT NULL,
	[Thread] [varchar](255) NOT NULL,
	[Host] [varchar](255) NOT NULL,
	[User] [varchar](255) NOT NULL,
	[Application] [varchar](255) NOT NULL,
	[Level] [varchar](50) NOT NULL,
	[Logger] [varchar](255) NOT NULL,
	[Message] [varchar](max) NOT NULL,
	[Exception] [varchar](max) NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [ETL].[Staging_BidContractDetail]    Script Date: 10/27/2016 1:05:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [ETL].[Staging_BidContractDetail](
	[Action] [varchar](1) NULL,
	[CompanyNumber] [varchar](3) NULL,
	[DivisionNumber] [varchar](3) NULL,
	[DepartmentNumber] [varchar](3) NULL,
	[BidNumber] [varchar](10) NULL,
	[ItemNumber] [varchar](10) NULL,
	[BidLineNumber] [varchar](5) NULL,
	[CategoryNumber] [varchar](5) NULL,
	[CategoryDescription] [varchar](40) NULL,
	[ForceEachOrCaseOnly] [varchar](1) NULL
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [ETL].[Staging_BidContractHeader]    Script Date: 10/27/2016 1:05:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [ETL].[Staging_BidContractHeader](
	[Action] [varchar](1) NULL,
	[CompanyNumber] [varchar](3) NULL,
	[DivisionNumber] [varchar](3) NULL,
	[DepartmentNumber] [varchar](3) NULL,
	[BidNumber] [varchar](10) NULL,
	[BidDescription] [varchar](40) NULL
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [ETL].[Staging_Branch]    Script Date: 10/27/2016 1:05:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [ETL].[Staging_Branch](
	[BranchId] [char](10) NULL,
	[LocationTypeId] [bigint] NULL,
	[Description] [varchar](50) NULL
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [ETL].[Staging_BrandControlLabels]    Script Date: 10/27/2016 1:05:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [ETL].[Staging_BrandControlLabels](
	[ControlLabel] [varchar](10) NULL,
	[ExtendedDescription] [varchar](30) NULL
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [ETL].[Staging_Brands]    Script Date: 10/27/2016 1:05:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [ETL].[Staging_Brands](
	[Brand] [varchar](20) NULL,
	[ControlLabel] [varchar](10) NULL,
	[ExtendedDescription] [varchar](30) NULL
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [ETL].[Staging_Category]    Script Date: 10/27/2016 1:05:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [ETL].[Staging_Category](
	[CategoryId] [char](6) NULL,
	[CategoryName] [varchar](50) NULL,
	[PPICode] [char](8) NULL
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [ETL].[Staging_Customer]    Script Date: 10/27/2016 1:05:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [ETL].[Staging_Customer](
	[Action] [varchar](1) NULL,
	[CO] [varchar](3) NULL,
	[DIV] [varchar](3) NULL,
	[DEPT] [varchar](3) NULL,
	[CustomerNumber] [varchar](10) NULL,
	[CustomerName] [varchar](40) NULL,
	[Address1] [varchar](25) NULL,
	[Address2] [varchar](25) NULL,
	[City] [varchar](30) NULL,
	[State] [varchar](3) NULL,
	[ZipCode] [varchar](10) NULL,
	[WHSNNumber] [varchar](3) NULL,
	[Telephone] [varchar](10) NULL,
	[SalesRep] [varchar](3) NULL,
	[ChainStoreCode] [varchar](10) NULL,
	[TermCode] [varchar](3) NULL,
	[CreditLimit] [varchar](7) NULL,
	[CreditHoldFlag] [varchar](1) NULL,
	[StoreNumber] [varchar](10) NULL,
	[ContractOnly] [varchar](1) NULL,
	[LocationAuth] [varchar](1) NULL,
	[DateOfLastPayment] [varchar](50) NULL,
	[AmountDue] [varchar](16) NULL,
	[CurrentBalance] [varchar](16) NULL,
	[PDACXAge1] [varchar](16) NULL,
	[PDACXAge2] [varchar](16) NULL,
	[PDACXAge3] [varchar](16) NULL,
	[PDACXAge4] [varchar](16) NULL,
	[ActiveFlag] [varchar](1) NULL,
	[MondayRoutNum] [varchar](5) NULL,
	[MondayStopNum] [varchar](3) NULL,
	[TuesdayRoutNum] [varchar](5) NULL,
	[TuesdayStopNum] [varchar](3) NULL,
	[WednesdayRoutNum] [varchar](5) NULL,
	[WednesdayStopNum] [varchar](3) NULL,
	[ThursdayRoutNum] [varchar](5) NULL,
	[ThursdayStopNum] [varchar](3) NULL,
	[FridayRoutNum] [varchar](5) NULL,
	[FridayStopNum] [varchar](3) NULL,
	[SaturdayRoutNum] [varchar](5) NULL,
	[SaturdayStopNum] [varchar](3) NULL,
	[SundayRoutNum] [varchar](5) NULL,
	[SundayStopNum] [varchar](3) NULL,
	[CSR] [varchar](3) NULL,
	[Contract] [varchar](10) NULL,
	[PORequiredFlag] [varchar](1) NULL,
	[JDCNTY1] [varchar](3) NULL,
	[JDCNTY2] [varchar](3) NULL,
	[PowerMenu] [varchar](1) NULL,
	[AchType] [char](1) NULL,
	[DsmNumber] [char](2) NULL,
	[NaId] [varchar](5) NULL,
	[NaNumber] [varchar](2) NULL,
	[NaSub] [varchar](2) NULL,
	[RaId] [varchar](5) NULL,
	[RaNumber] [varchar](4) NULL,
	[IsKeithnetCustomer] [char](1) NULL
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [ETL].[Staging_CustomerBid]    Script Date: 10/27/2016 1:05:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [ETL].[Staging_CustomerBid](
	[Action] [varchar](1) NULL,
	[CompanyNumber] [varchar](3) NULL,
	[DivisionNumber] [varchar](3) NULL,
	[DepartmentNumber] [varchar](3) NULL,
	[CustomerNumber] [varchar](10) NULL,
	[PriorityNumber] [varchar](3) NULL,
	[BidNumber] [varchar](10) NULL
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [ETL].[Staging_Dsr]    Script Date: 10/27/2016 1:05:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [ETL].[Staging_Dsr](
	[DsrNumber] [char](8) NOT NULL,
	[Name] [varchar](50) NOT NULL,
	[EmailAddress] [varchar](50) NOT NULL
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [ETL].[Staging_EmployeeInfo]    Script Date: 10/27/2016 1:05:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [ETL].[Staging_EmployeeInfo](
	[EMPLID] [varchar](11) NOT NULL,
	[EMAIL_ADDR] [varchar](70) NULL,
	[PHONE] [varchar](24) NULL,
	[EMPLOYEE_PHOTO] [varbinary](max) NULL,
	[LAST_NAME] [varchar](30) NULL,
	[FIRST_NAME] [varchar](30) NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [ETL].[Staging_FSE_ProductAllergens]    Script Date: 10/27/2016 1:05:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING OFF
GO
CREATE TABLE [ETL].[Staging_FSE_ProductAllergens](
	[Gtin] [char](14) NULL
) ON [PRIMARY]
SET ANSI_PADDING ON
ALTER TABLE [ETL].[Staging_FSE_ProductAllergens] ADD [AllergenTypeCode] [varchar](10) NULL
SET ANSI_PADDING OFF
ALTER TABLE [ETL].[Staging_FSE_ProductAllergens] ADD [AllergenTypeDesc] [varchar](50) NULL
ALTER TABLE [ETL].[Staging_FSE_ProductAllergens] ADD [LevelOfContainment] [varchar](20) NULL

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [ETL].[Staging_FSE_ProductDiet]    Script Date: 10/27/2016 1:05:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING OFF
GO
CREATE TABLE [ETL].[Staging_FSE_ProductDiet](
	[Gtin] [char](14) NULL,
	[DietType] [varchar](25) NULL,
	[Value] [char](1) NULL
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [ETL].[Staging_FSE_ProductNutrition]    Script Date: 10/27/2016 1:05:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING OFF
GO
CREATE TABLE [ETL].[Staging_FSE_ProductNutrition](
	[Gtin] [char](14) NULL,
	[NutrientTypeCode] [varchar](100) NULL,
	[NutrientTypeDesc] [varchar](150) NULL,
	[MeasurmentTypeId] [varchar](5) NULL,
	[MeasurementValue] [decimal](20, 3) NULL,
	[DailyValue] [varchar](100) NULL
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [ETL].[Staging_FSE_ProductSpec]    Script Date: 10/27/2016 1:05:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING OFF
GO
CREATE TABLE [ETL].[Staging_FSE_ProductSpec](
	[Gtin] [char](14) NULL,
	[PackageGtin] [char](14) NULL,
	[BekItemNumber] [char](6) NULL,
	[Active] [bit] NULL,
	[ProductType] [varchar](20) NULL,
	[ProductShortDesc] [varchar](70) NULL,
	[ProductAdditionalDesc] [varchar](max) NULL,
	[ManufacturerItemNumber] [varchar](30) NULL,
	[UnitsPerCase] [int] NULL,
	[UnitMeasure] [decimal](20, 3) NULL,
	[UnitMeasureUOM] [varchar](3) NULL,
	[GrossWeight] [decimal](20, 3) NULL,
	[NetWeight] [decimal](20, 3) NULL,
	[Length] [decimal](20, 3) NULL,
	[Width] [decimal](20, 3) NULL,
	[Height] [decimal](20, 3) NULL,
	[Volume] [decimal](20, 3) NULL,
	[TiHi] [varchar](250) NULL,
	[Shelf] [int] NULL,
	[StorageTemp] [varchar](35) NULL,
	[ServingsPerPack] [int] NULL,
	[ServingSuggestion] [varchar](max) NULL,
	[MoreInformation] [varchar](35) NULL,
	[MarketingMessage] [varchar](max) NULL,
	[ServingSize] [decimal](20, 3) NULL,
	[ServingSizeUOM] [varchar](3) NULL,
	[Ingredients] [varchar](max) NULL,
	[Brand] [varchar](35) NULL,
	[BrandOwner] [varchar](max) NULL,
	[CountryOfOrigin] [varchar](100) NULL,
	[ContryOfOriginName] [varchar](100) NULL,
	[PreparationInstructions] [varchar](max) NULL,
	[HandlingInstruction] [varchar](max) NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [ETL].[Staging_KNet_Invoice]    Script Date: 10/27/2016 1:05:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [ETL].[Staging_KNet_Invoice](
	[Action] [varchar](1) NULL,
	[CompanyNumber] [varchar](3) NULL,
	[DivisionNumber] [varchar](3) NULL,
	[DepartmentNumber] [varchar](3) NULL,
	[CustomerNumber] [varchar](10) NULL,
	[OrderNumber] [varchar](9) NULL,
	[LineNumber] [varchar](5) NULL,
	[MemoBillCode] [varchar](3) NULL,
	[CreditOFlag] [varchar](1) NULL,
	[TradeSWFlag] [varchar](1) NULL,
	[ShipDate] [varchar](8) NULL,
	[OrderDate] [varchar](8) NULL,
	[RouteNumber] [varchar](5) NULL,
	[StopNumber] [varchar](3) NULL,
	[WHNumber] [varchar](3) NULL,
	[ItemNumber] [varchar](10) NULL,
	[QuantityOrdered] [varchar](7) NULL,
	[QuantityShipped] [varchar](7) NULL,
	[BrokenCaseCode] [varchar](1) NULL,
	[CatchWeightCode] [varchar](1) NULL,
	[ExtCatchWeight] [varchar](12) NULL,
	[ItemPrice] [varchar](10) NULL,
	[PriceBookNumber] [varchar](5) NULL,
	[ItemPriceSRP] [varchar](12) NULL,
	[OriginalInvoiceNumber] [varchar](20) NULL,
	[InvoiceNumber] [varchar](20) NULL,
	[AC] [varchar](1) NULL,
	[ChangeDate] [varchar](8) NULL,
	[DateOfLastOrder] [varchar](8) NULL,
	[ExtSRPAmount] [varchar](12) NULL,
	[ExtSalesGross] [varchar](16) NULL,
	[ExtSalesNet] [varchar](16) NULL,
	[CustomerGroup] [varchar](10) NULL,
	[SalesRep] [varchar](3) NULL,
	[VendorNumber] [varchar](10) NULL,
	[CustomerPO] [varchar](20) NULL,
	[ChainStoreCode] [varchar](10) NULL,
	[CombStatementCustomer] [varchar](10) NULL,
	[PriceBook] [varchar](7) NULL,
	[ClassCode] [varchar](5) NULL
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [ETL].[Staging_KPay_Invoice]    Script Date: 10/27/2016 1:05:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [ETL].[Staging_KPay_Invoice](
	[InvoiceNumber] [varchar](30) NOT NULL,
	[Division] [char](5) NOT NULL,
	[CustomerNumber] [char](6) NOT NULL,
	[ItemSequence] [smallint] NOT NULL,
	[InvoiceType] [char](3) NOT NULL,
	[InvoiceDate] [datetime] NOT NULL,
	[DueDate] [datetime] NOT NULL,
	[AmountDue] [decimal](9, 2) NOT NULL,
	[DeleteFlag] [bit] NOT NULL
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [ETL].[Staging_NationalIdDesc]    Script Date: 10/27/2016 1:05:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [ETL].[Staging_NationalIdDesc](
	[NationalId] [varchar](5) NULL,
	[NationalIdDesc] [varchar](46) NULL
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [ETL].[Staging_NationalNumberAndSubDesc]    Script Date: 10/27/2016 1:05:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [ETL].[Staging_NationalNumberAndSubDesc](
	[NationalNumber] [varchar](2) NULL,
	[NationalSub] [varchar](2) NULL,
	[NationalNumberAndSubDesc] [varchar](46) NULL
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [ETL].[Staging_OpenDetailAR]    Script Date: 10/27/2016 1:05:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [ETL].[Staging_OpenDetailAR](
	[Action] [varchar](50) NULL,
	[Company] [varchar](50) NULL,
	[Division] [varchar](50) NULL,
	[Department] [varchar](3) NULL,
	[Customer] [varchar](10) NULL,
	[OriginalInvoiceNumber] [varchar](20) NULL,
	[InvoiceNumber] [varchar](20) NULL,
	[AC] [varchar](1) NULL,
	[ChangeDate] [varchar](8) NULL,
	[ReclineNumber] [varchar](9) NULL,
	[InvoiceType] [varchar](1) NULL,
	[DateOfLastOrder] [varchar](8) NULL,
	[InvoiceAmount] [varchar](16) NULL,
	[CheckNumber] [varchar](9) NULL,
	[AdjustCode] [varchar](3) NULL,
	[InvoiceReference] [varchar](20) NULL,
	[InvoiceDue] [varchar](8) NULL,
	[CombinedStmt] [varchar](10) NULL
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [ETL].[Staging_PaidDetail]    Script Date: 10/27/2016 1:05:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [ETL].[Staging_PaidDetail](
	[Action] [varchar](50) NULL,
	[AdjustCode] [varchar](50) NULL,
	[Company] [varchar](50) NULL,
	[Division] [varchar](50) NULL,
	[Department] [varchar](3) NULL,
	[Customer] [varchar](10) NULL,
	[InvoiceNumber] [varchar](20) NULL,
	[ReclineNumber] [varchar](9) NULL,
	[InvoiceType] [varchar](1) NULL,
	[DateOfLastOrder] [varchar](8) NULL,
	[InvoiceAmount] [varchar](16) NULL,
	[CheckNumber] [varchar](9) NULL,
	[InvoiceReference] [varchar](20) NULL,
	[InvoiceDue] [varchar](8) NULL,
	[CombinedStmt] [varchar](10) NULL
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [ETL].[Staging_ProprietaryCustomer]    Script Date: 10/27/2016 1:05:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [ETL].[Staging_ProprietaryCustomer](
	[Action] [varchar](1) NULL,
	[CompanyNumber] [varchar](3) NULL,
	[DivisionNumber] [varchar](3) NULL,
	[DepartmentNumber] [varchar](3) NULL,
	[ProprietaryNumber] [varchar](10) NULL,
	[CustomerNumber] [varchar](10) NULL
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [ETL].[Staging_ProprietaryItem]    Script Date: 10/27/2016 1:05:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [ETL].[Staging_ProprietaryItem](
	[Action] [varchar](1) NULL,
	[CompanyNumber] [varchar](3) NULL,
	[DivisionNumber] [varchar](3) NULL,
	[DepartmentNumber] [varchar](3) NULL,
	[ProprietaryNumber] [varchar](10) NULL,
	[ItemNumber] [varchar](10) NULL
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [ETL].[Staging_RegionalIdDesc]    Script Date: 10/27/2016 1:05:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [ETL].[Staging_RegionalIdDesc](
	[RegionalId] [varchar](5) NULL,
	[RegionalIdDesc] [varchar](45) NULL
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [ETL].[Staging_RegionalNumberDesc]    Script Date: 10/27/2016 1:05:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [ETL].[Staging_RegionalNumberDesc](
	[RegionalNumber] [varchar](4) NULL,
	[RegionalNumberDesc] [varchar](46) NULL
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [ETL].[Staging_Terms]    Script Date: 10/27/2016 1:05:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [ETL].[Staging_Terms](
	[Action] [varchar](1) NULL,
	[Company] [varchar](3) NULL,
	[Code] [varchar](3) NULL,
	[Description] [varchar](25) NULL,
	[Age1] [varchar](3) NULL,
	[Age2] [varchar](3) NULL,
	[Age3] [varchar](3) NULL,
	[Age4] [varchar](3) NULL,
	[Prox] [varchar](1) NULL
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [ETL].[Staging_UNFIProducts]    Script Date: 10/27/2016 1:05:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [ETL].[Staging_UNFIProducts](
	[WarehouseNumber] [int] NULL,
	[Description] [varchar](150) NULL,
	[Brand] [varchar](150) NULL,
	[CLength] [decimal](18, 0) NULL,
	[CWidth] [decimal](9, 3) NULL,
	[CHeight] [decimal](9, 3) NULL,
	[TempControl] [varchar](50) NULL,
	[UnitOfSale] [varchar](50) NULL,
	[CatalogDept] [varchar](50) NULL,
	[ShipMinExpire] [varchar](50) NULL,
	[ProductNumber] [varchar](10) NULL,
	[MinOrder] [int] NULL,
	[VendorCasesPerTier] [int] NULL,
	[VendorTiersPerPallet] [int] NULL,
	[VendorCasesPerPallet] [int] NULL,
	[CaseQuantity] [int] NULL,
	[PutUp] [varchar](50) NULL,
	[ContSize] [decimal](9, 3) NULL,
	[ContUnit] [varchar](50) NULL,
	[TCSCode] [varchar](50) NULL,
	[RetailUPC] [varchar](50) NULL,
	[CaseUPC] [varchar](50) NULL,
	[Weight] [decimal](9, 3) NULL,
	[PLength] [decimal](9, 3) NULL,
	[PHeight] [decimal](9, 3) NULL,
	[PWidth] [decimal](9, 3) NULL,
	[Status] [varchar](50) NULL,
	[Type] [varchar](50) NULL,
	[Category] [varchar](50) NULL,
	[Subgroup] [varchar](50) NULL,
	[EachPrice] [decimal](9, 2) NULL,
	[CasePrice] [decimal](9, 2) NULL,
	[Flag1] [varchar](50) NULL,
	[Flag2] [varchar](50) NULL,
	[Flag3] [varchar](50) NULL,
	[Flag4] [varchar](50) NULL,
	[OnHandQty] [int] NULL,
	[Vendor] [varchar](150) NULL,
	[StockedInBranches] [varchar](50) NULL
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [ETL].[Staging_WorksheetItems]    Script Date: 10/27/2016 1:05:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [ETL].[Staging_WorksheetItems](
	[Action] [varchar](1) NULL,
	[CompanyNumber] [varchar](3) NULL,
	[DivisionNumber] [varchar](3) NULL,
	[DepartmentNumber] [varchar](3) NULL,
	[CustomerNumber] [varchar](10) NULL,
	[ItemNumber] [varchar](10) NULL,
	[BrokenCaseCode] [varchar](1) NULL,
	[ItemPrice] [varchar](10) NULL,
	[QtyOrdered] [varchar](7) NULL,
	[DateOfLastOrder] [varchar](8) NULL
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [Invoice].[InvoiceItems]    Script Date: 10/27/2016 1:05:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [Invoice].[InvoiceItems](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[QuantityOrdered] [int] NULL,
	[QuantityShipped] [int] NULL,
	[CatchWeightCode] [bit] NOT NULL,
	[ExtCatchWeight] [decimal](18, 2) NULL,
	[ItemPrice] [decimal](18, 2) NULL,
	[ExtSalesNet] [decimal](18, 2) NULL,
	[CreatedUtc] [datetime] NOT NULL,
	[ModifiedUtc] [datetime] NOT NULL,
	[InvoiceId] [bigint] NOT NULL,
	[ItemNumber] [nvarchar](10) NULL,
	[ClassCode] [char](2) NULL,
	[LineNumber] [nvarchar](6) NULL,
 CONSTRAINT [PK_Invoice.InvoiceItems] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [Invoice].[Invoices]    Script Date: 10/27/2016 1:05:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [Invoice].[Invoices](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[InvoiceNumber] [nvarchar](max) NULL,
	[OrderDate] [datetime] NULL,
	[CustomerNumber] [nvarchar](max) NULL,
	[CreatedUtc] [datetime] NOT NULL,
	[ModifiedUtc] [datetime] NOT NULL,
	[BranchId] [nvarchar](3) NULL,
	[InvoiceDate] [datetime] NULL,
	[Type] [int] NOT NULL,
	[Amount] [decimal](18, 2) NOT NULL,
	[Status] [int] NOT NULL,
	[DueDate] [datetime] NULL,
 CONSTRAINT [PK_Invoice.Invoices] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [Invoice].[Terms]    Script Date: 10/27/2016 1:05:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [Invoice].[Terms](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[BranchId] [nvarchar](3) NULL,
	[TermCode] [int] NOT NULL,
	[Description] [nvarchar](25) NULL,
	[Age1] [int] NOT NULL,
	[Age2] [int] NOT NULL,
	[Age3] [int] NOT NULL,
	[Age4] [int] NOT NULL,
	[CreatedUtc] [datetime] NOT NULL,
	[ModifiedUtc] [datetime] NOT NULL,
 CONSTRAINT [PK_Invoice.Terms] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [List].[ListItems]    Script Date: 10/27/2016 1:05:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [List].[ListItems](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[ItemNumber] [nvarchar](15) NOT NULL,
	[Label] [nvarchar](150) NULL,
	[Par] [decimal](18, 2) NOT NULL,
	[Note] [nvarchar](200) NULL,
	[CreatedUtc] [datetime] NOT NULL DEFAULT (getutcdate()),
	[ParentList_Id] [bigint] NULL,
	[ModifiedUtc] [datetime] NOT NULL DEFAULT (getutcdate()),
	[Category] [nvarchar](40) NULL,
	[Position] [int] NOT NULL DEFAULT ((0)),
	[FromDate] [datetime] NULL,
	[ToDate] [datetime] NULL,
	[Each] [bit] NULL,
	[Quantity] [decimal](18, 2) NOT NULL DEFAULT ((0)),
	[CatalogId] [nvarchar](24) NULL,
 CONSTRAINT [PK_List.ListItems] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 80) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [List].[Lists]    Script Date: 10/27/2016 1:05:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [List].[Lists](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[UserId] [uniqueidentifier] NULL,
	[DisplayName] [nvarchar](max) NULL,
	[Type] [int] NOT NULL,
	[CustomerId] [nvarchar](10) NULL,
	[BranchId] [nvarchar](10) NULL,
	[AccountNumber] [nvarchar](max) NULL,
	[ReadOnly] [bit] NOT NULL,
	[CreatedUtc] [datetime] NOT NULL DEFAULT (getutcdate()),
	[ModifiedUtc] [datetime] NOT NULL DEFAULT (getutcdate()),
 CONSTRAINT [PK_List.Lists] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [List].[ListShares]    Script Date: 10/27/2016 1:05:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [List].[ListShares](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[CustomerId] [nvarchar](max) NULL,
	[BranchId] [nvarchar](max) NULL,
	[CreatedUtc] [datetime] NOT NULL DEFAULT (getutcdate()),
	[ModifiedUtc] [datetime] NOT NULL DEFAULT (getutcdate()),
	[SharedList_Id] [bigint] NULL,
 CONSTRAINT [PK_List.ListShares] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [Messaging].[CustomerTopics]    Script Date: 10/27/2016 1:05:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [Messaging].[CustomerTopics](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[CustomerNumber] [varchar](9) NULL,
	[ProviderTopicId] [varchar](255) NULL,
	[NotificationType] [int] NOT NULL,
	[CreatedUtc] [datetime] NOT NULL,
	[ModifiedUtc] [datetime] NOT NULL,
 CONSTRAINT [PK_Messaging.CustomerTopics] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [Messaging].[UserMessages]    Script Date: 10/27/2016 1:05:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [Messaging].[UserMessages](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[CustomerNumber] [varchar](9) NULL,
	[UserId] [uniqueidentifier] NOT NULL,
	[NotificationType] [int] NOT NULL,
	[MessageReadUtc] [datetime] NULL,
	[CreatedUtc] [datetime] NOT NULL DEFAULT (getutcdate()),
	[ModifiedUtc] [datetime] NOT NULL DEFAULT (getutcdate()),
	[Body] [nvarchar](max) NULL,
	[Subject] [nvarchar](max) NULL,
	[Mandatory] [bit] NOT NULL DEFAULT ((0)),
	[Label] [nvarchar](max) NULL,
	[CustomerName] [nvarchar](250) NULL,
	[BranchId] [nvarchar](3) NULL,
 CONSTRAINT [PK_Messaging.UserMessages] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [Messaging].[UserMessagingPreferences]    Script Date: 10/27/2016 1:05:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [Messaging].[UserMessagingPreferences](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[UserId] [uniqueidentifier] NOT NULL,
	[NotificationType] [int] NOT NULL,
	[Channel] [int] NOT NULL,
	[CustomerNumber] [varchar](9) NULL,
	[CreatedUtc] [datetime] NOT NULL DEFAULT (getutcdate()),
	[ModifiedUtc] [datetime] NOT NULL DEFAULT (getutcdate()),
	[BranchId] [varchar](4) NULL,
 CONSTRAINT [PK_Messaging.UserMessagingPreferences] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [Messaging].[UserPushNotificationDevices]    Script Date: 10/27/2016 1:05:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [Messaging].[UserPushNotificationDevices](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[UserId] [uniqueidentifier] NOT NULL,
	[DeviceId] [nvarchar](255) NOT NULL,
	[ProviderToken] [nvarchar](255) NOT NULL,
	[ProviderEndpointId] [nvarchar](255) NULL,
	[DeviceOS] [int] NOT NULL,
	[CreatedUtc] [datetime] NOT NULL DEFAULT (getutcdate()),
	[ModifiedUtc] [datetime] NOT NULL DEFAULT (getutcdate()),
	[Enabled] [bit] NULL,
 CONSTRAINT [PK_Messaging.UserPushNotificationDevices] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [Messaging].[UserTopicSubscriptions]    Script Date: 10/27/2016 1:05:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [Messaging].[UserTopicSubscriptions](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[UserId] [uniqueidentifier] NOT NULL,
	[ProviderSubscriptionId] [varchar](255) NULL,
	[NotificationEndpoint] [varchar](255) NULL,
	[CreatedUtc] [datetime] NOT NULL,
	[ModifiedUtc] [datetime] NOT NULL,
	[CustomerTopic_Id] [bigint] NULL,
	[Channel] [int] NOT NULL,
 CONSTRAINT [PK_Messaging.UserTopicSubscriptions] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [Orders].[IdentityCounter]    Script Date: 10/27/2016 1:05:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [Orders].[IdentityCounter](
	[CounterName] [nvarchar](50) NULL,
	[StartId] [int] NULL,
	[EndId] [int] NULL,
	[CurrentId] [int] NULL
) ON [PRIMARY]

GO
/****** Object:  Table [Orders].[OrderHistoryDetail]    Script Date: 10/27/2016 1:05:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [Orders].[OrderHistoryDetail](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[ItemNumber] [char](6) NULL,
	[LineNumber] [int] NOT NULL,
	[OrderQuantity] [int] NOT NULL,
	[ShippedQuantity] [int] NOT NULL,
	[UnitOfMeasure] [char](1) NULL,
	[CatchWeight] [bit] NOT NULL,
	[ItemDeleted] [bit] NOT NULL,
	[SubbedOriginalItemNumber] [char](6) NULL,
	[ReplacedOriginalItemNumber] [char](6) NULL,
	[ItemStatus] [char](1) NULL,
	[TotalShippedWeight] [decimal](18, 2) NOT NULL,
	[CreatedUtc] [datetime] NOT NULL DEFAULT (getutcdate()),
	[ModifiedUtc] [datetime] NOT NULL DEFAULT (getutcdate()),
	[OrderHistoryHeader_Id] [bigint] NULL,
	[BranchId] [char](3) NULL,
	[InvoiceNumber] [varchar](10) NULL,
	[SellPrice] [decimal](18, 2) NOT NULL DEFAULT ((0)),
	[Source] [char](3) NULL,
	[ManufacturerId] [nvarchar](25) NULL,
	[SpecialOrderHeaderId] [char](7) NULL,
	[SpecialOrderLineNumber] [char](3) NULL,
 CONSTRAINT [PK_Orders.OrderHistoryDetail] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [Orders].[OrderHistoryHeader]    Script Date: 10/27/2016 1:05:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [Orders].[OrderHistoryHeader](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[OrderSystem] [char](1) NULL,
	[BranchId] [char](3) NULL,
	[CustomerNumber] [char](6) NULL,
	[InvoiceNumber] [varchar](10) NULL,
	[PONumber] [nvarchar](20) NULL,
	[ControlNumber] [char](7) NULL,
	[OrderStatus] [char](1) NULL,
	[FutureItems] [bit] NOT NULL,
	[ErrorStatus] [bit] NOT NULL,
	[RouteNumber] [char](4) NULL,
	[CreatedUtc] [datetime] NOT NULL DEFAULT (getutcdate()),
	[ModifiedUtc] [datetime] NOT NULL DEFAULT (getutcdate()),
	[StopNumber] [char](3) NULL,
	[DeliveryOutOfSequence] [bit] NULL,
	[OriginalControlNumber] [char](7) NULL,
	[IsSpecialOrder] [bit] NOT NULL DEFAULT ((0)),
	[RelatedControlNumber] [char](7) NULL,
	[DeliveryDate] [char](10) NULL,
	[ScheduledDeliveryTime] [char](19) NULL,
	[EstimatedDeliveryTime] [char](19) NULL,
	[ActualDeliveryTime] [char](19) NULL,
	[OrderSubtotal] [decimal](18, 2) NOT NULL DEFAULT ((0)),
 CONSTRAINT [PK_Orders.OrderHistoryHeader] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 80) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [Orders].[UserActiveCarts]    Script Date: 10/27/2016 1:05:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [Orders].[UserActiveCarts](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[UserId] [uniqueidentifier] NOT NULL,
	[CartId] [uniqueidentifier] NOT NULL,
	[CreatedUtc] [datetime] NOT NULL DEFAULT (getutcdate()),
	[ModifiedUtc] [datetime] NOT NULL DEFAULT (getutcdate()),
	[CustomerId] [nvarchar](max) NULL,
	[BranchId] [nvarchar](max) NULL,
 CONSTRAINT [PK_Orders.UserActiveCarts] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [Profile].[MarketingPreferences]    Script Date: 10/27/2016 1:05:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [Profile].[MarketingPreferences](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[Email] [nvarchar](150) NULL,
	[BranchId] [nvarchar](max) NULL,
	[CurrentCustomer] [bit] NOT NULL,
	[LearnMore] [bit] NOT NULL,
	[RegisteredOn] [datetime] NOT NULL,
	[CreatedUtc] [datetime] NOT NULL,
	[ModifiedUtc] [datetime] NOT NULL,
 CONSTRAINT [PK_Profile.MarketingPreferences] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [Profile].[PasswordResetRequests]    Script Date: 10/27/2016 1:05:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [Profile].[PasswordResetRequests](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[UserId] [uniqueidentifier] NOT NULL,
	[Token] [nvarchar](300) NULL,
	[Expiration] [datetime] NOT NULL,
	[Processed] [bit] NOT NULL,
	[CreatedUtc] [datetime] NOT NULL DEFAULT (getutcdate()),
	[ModifiedUtc] [datetime] NOT NULL DEFAULT (getutcdate()),
 CONSTRAINT [PK_Profile.PasswordResetRequests] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [Profile].[Settings]    Script Date: 10/27/2016 1:05:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [Profile].[Settings](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[UserId] [uniqueidentifier] NOT NULL,
	[Key] [varchar](100) NOT NULL,
	[Value] [varchar](250) NOT NULL,
	[CreatedUtc] [datetime] NOT NULL DEFAULT (getutcdate()),
	[ModifiedUtc] [datetime] NOT NULL DEFAULT (getutcdate()),
 CONSTRAINT [PK_Profile.Settings] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Index [PK_Log]    Script Date: 10/27/2016 1:05:25 PM ******/
CREATE UNIQUE CLUSTERED INDEX [PK_Log] ON [dbo].[Log]
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_UserId]    Script Date: 10/27/2016 1:05:25 PM ******/
CREATE NONCLUSTERED INDEX [IX_UserId] ON [BranchSupport].[DsrAliases]
(
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [IX_AppSettings_Key]    Script Date: 10/27/2016 1:05:25 PM ******/
CREATE NONCLUSTERED INDEX [IX_AppSettings_Key] ON [Configuration].[AppSettings]
(
	[Key] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [IX_TemplateKey]    Script Date: 10/27/2016 1:05:25 PM ******/
CREATE NONCLUSTERED INDEX [IX_TemplateKey] ON [Configuration].[MessageTemplates]
(
	[TemplateKey] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [IdxByEmailAddress]    Script Date: 10/27/2016 1:05:25 PM ******/
CREATE NONCLUSTERED INDEX [IdxByEmailAddress] ON [Customers].[InternalUserAccess]
(
	[EmailAddress] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [IdxInternalUser]    Script Date: 10/27/2016 1:05:25 PM ******/
CREATE NONCLUSTERED INDEX [IdxInternalUser] ON [Customers].[InternalUserAccess]
(
	[BranchId] ASC,
	[CustomerNumber] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 80) ON [PRIMARY]
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [IdxItemHistory]    Script Date: 10/27/2016 1:05:25 PM ******/
CREATE NONCLUSTERED INDEX [IdxItemHistory] ON [Customers].[ItemHistory]
(
	[BranchId] ASC,
	[CustomerNumber] ASC,
	[ItemNumber] ASC,
	[UnitOfMeasure] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_Log_Date]    Script Date: 10/27/2016 1:05:25 PM ******/
CREATE NONCLUSTERED INDEX [IX_Log_Date] ON [dbo].[Log]
(
	[Date] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [ix_INBFCF]    Script Date: 10/27/2016 1:05:25 PM ******/
CREATE NONCLUSTERED INDEX [ix_INBFCF] ON [ETL].[Staging_BidContractDetail]
(
	[DivisionNumber] ASC,
	[BidNumber] ASC
)
INCLUDE ( 	[ItemNumber],
	[BidLineNumber],
	[CategoryDescription],
	[ForceEachOrCaseOnly]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [IX_Stg_CustBid_BidNumver_DivisionNumber]    Script Date: 10/27/2016 1:05:25 PM ******/
CREATE NONCLUSTERED INDEX [IX_Stg_CustBid_BidNumver_DivisionNumber] ON [ETL].[Staging_CustomerBid]
(
	[BidNumber] ASC,
	[DivisionNumber] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 80) ON [PRIMARY]
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [IX_CustDiv]    Script Date: 10/27/2016 1:05:25 PM ******/
CREATE NONCLUSTERED INDEX [IX_CustDiv] ON [ETL].[Staging_WorksheetItems]
(
	[DivisionNumber] ASC,
	[CustomerNumber] ASC
)
INCLUDE ( 	[ItemNumber],
	[BrokenCaseCode]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_InvoiceId]    Script Date: 10/27/2016 1:05:25 PM ******/
CREATE NONCLUSTERED INDEX [IX_InvoiceId] ON [Invoice].[InvoiceItems]
(
	[InvoiceId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [IX_ItemParent]    Script Date: 10/27/2016 1:05:25 PM ******/
CREATE NONCLUSTERED INDEX [IX_ItemParent] ON [List].[ListItems]
(
	[ItemNumber] ASC,
	[ParentList_Id] ASC,
	[Each] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 80) ON [PRIMARY]
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [ix_ListItems_ParentListId_Include]    Script Date: 10/27/2016 1:05:25 PM ******/
CREATE NONCLUSTERED INDEX [ix_ListItems_ParentListId_Include] ON [List].[ListItems]
(
	[ParentList_Id] ASC
)
INCLUDE ( 	[ItemNumber],
	[Label],
	[Par],
	[Note],
	[CreatedUtc],
	[ModifiedUtc],
	[Category],
	[Position],
	[FromDate],
	[ToDate],
	[Each],
	[Quantity],
	[CatalogId]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [IX_ParentId]    Script Date: 10/27/2016 1:05:25 PM ******/
CREATE NONCLUSTERED INDEX [IX_ParentId] ON [List].[ListItems]
(
	[ParentList_Id] ASC
)
INCLUDE ( 	[ItemNumber],
	[Category],
	[Position],
	[Each]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 80) ON [PRIMARY]
GO
/****** Object:  Index [IX_ParentList_Id]    Script Date: 10/27/2016 1:05:25 PM ******/
CREATE NONCLUSTERED INDEX [IX_ParentList_Id] ON [List].[ListItems]
(
	[ParentList_Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 80) ON [PRIMARY]
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [IX_CustBranch]    Script Date: 10/27/2016 1:05:25 PM ******/
CREATE NONCLUSTERED INDEX [IX_CustBranch] ON [List].[Lists]
(
	[CustomerId] ASC,
	[BranchId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_Type]    Script Date: 10/27/2016 1:05:25 PM ******/
CREATE NONCLUSTERED INDEX [IX_Type] ON [List].[Lists]
(
	[Type] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_SharedList_Id]    Script Date: 10/27/2016 1:05:25 PM ******/
CREATE NONCLUSTERED INDEX [IX_SharedList_Id] ON [List].[ListShares]
(
	[SharedList_Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [idx_UserId]    Script Date: 10/27/2016 1:05:25 PM ******/
CREATE NONCLUSTERED INDEX [idx_UserId] ON [Messaging].[UserMessages]
(
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [idx_UserId_ReadDateUtc]    Script Date: 10/27/2016 1:05:25 PM ******/
CREATE NONCLUSTERED INDEX [idx_UserId_ReadDateUtc] ON [Messaging].[UserMessages]
(
	[UserId] ASC,
	[MessageReadUtc] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_UserId]    Script Date: 10/27/2016 1:05:25 PM ******/
CREATE NONCLUSTERED INDEX [IX_UserId] ON [Messaging].[UserPushNotificationDevices]
(
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_CustomerTopic_Id]    Script Date: 10/27/2016 1:05:25 PM ******/
CREATE NONCLUSTERED INDEX [IX_CustomerTopic_Id] ON [Messaging].[UserTopicSubscriptions]
(
	[CustomerTopic_Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [IdxItemUsageGrouping]    Script Date: 10/27/2016 1:05:25 PM ******/
CREATE NONCLUSTERED INDEX [IdxItemUsageGrouping] ON [Orders].[OrderHistoryDetail]
(
	[ItemNumber] ASC,
	[UnitOfMeasure] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 80) ON [PRIMARY]
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [IdxOrderDetail]    Script Date: 10/27/2016 1:05:25 PM ******/
CREATE NONCLUSTERED INDEX [IdxOrderDetail] ON [Orders].[OrderHistoryDetail]
(
	[BranchId] ASC,
	[InvoiceNumber] ASC,
	[LineNumber] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 80) ON [PRIMARY]
GO
/****** Object:  Index [IX_OrderHistoryHeader_Id]    Script Date: 10/27/2016 1:05:25 PM ******/
CREATE NONCLUSTERED INDEX [IX_OrderHistoryHeader_Id] ON [Orders].[OrderHistoryDetail]
(
	[OrderHistoryHeader_Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 80) ON [PRIMARY]
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [IdxCustomerNumberByDate]    Script Date: 10/27/2016 1:05:25 PM ******/
CREATE NONCLUSTERED INDEX [IdxCustomerNumberByDate] ON [Orders].[OrderHistoryHeader]
(
	[CustomerNumber] ASC,
	[DeliveryDate] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 80) ON [PRIMARY]
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [IdxOrderHeader]    Script Date: 10/27/2016 1:05:25 PM ******/
CREATE NONCLUSTERED INDEX [IdxOrderHeader] ON [Orders].[OrderHistoryHeader]
(
	[BranchId] ASC,
	[InvoiceNumber] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 80) ON [PRIMARY]
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [ix_OrderHistoryheader_OrderSystem_includes]    Script Date: 10/27/2016 1:05:25 PM ******/
CREATE NONCLUSTERED INDEX [ix_OrderHistoryheader_OrderSystem_includes] ON [Orders].[OrderHistoryHeader]
(
	[OrderSystem] ASC
)
INCLUDE ( 	[Id],
	[BranchId],
	[CustomerNumber],
	[InvoiceNumber],
	[PONumber],
	[ControlNumber],
	[OrderStatus],
	[FutureItems],
	[ErrorStatus],
	[RouteNumber],
	[CreatedUtc],
	[ModifiedUtc],
	[StopNumber],
	[DeliveryOutOfSequence],
	[OriginalControlNumber],
	[IsSpecialOrder],
	[RelatedControlNumber],
	[DeliveryDate],
	[ScheduledDeliveryTime],
	[EstimatedDeliveryTime],
	[ActualDeliveryTime],
	[OrderSubtotal]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 100) ON [PRIMARY]
GO
/****** Object:  Index [IX_UserId]    Script Date: 10/27/2016 1:05:25 PM ******/
CREATE NONCLUSTERED INDEX [IX_UserId] ON [Profile].[Settings]
(
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
ALTER TABLE [BranchSupport].[BranchSupports] ADD  DEFAULT (getutcdate()) FOR [CreatedUtc]
GO
ALTER TABLE [BranchSupport].[BranchSupports] ADD  DEFAULT (getutcdate()) FOR [ModifiedUtc]
GO
ALTER TABLE [BranchSupport].[DsrAliases] ADD  DEFAULT (getutcdate()) FOR [CreatedUtc]
GO
ALTER TABLE [BranchSupport].[DsrAliases] ADD  DEFAULT (getutcdate()) FOR [ModifiedUtc]
GO
ALTER TABLE [BranchSupport].[DsrAliases] ADD  DEFAULT ('') FOR [DsrNumber]
GO
ALTER TABLE [Invoice].[InvoiceItems] ADD  DEFAULT (getutcdate()) FOR [CreatedUtc]
GO
ALTER TABLE [Invoice].[InvoiceItems] ADD  DEFAULT (getutcdate()) FOR [ModifiedUtc]
GO
ALTER TABLE [Invoice].[Invoices] ADD  DEFAULT (getutcdate()) FOR [CreatedUtc]
GO
ALTER TABLE [Invoice].[Invoices] ADD  DEFAULT (getutcdate()) FOR [ModifiedUtc]
GO
ALTER TABLE [Invoice].[Invoices] ADD  DEFAULT ((0)) FOR [Type]
GO
ALTER TABLE [Invoice].[Invoices] ADD  DEFAULT ((0)) FOR [Amount]
GO
ALTER TABLE [Invoice].[Invoices] ADD  DEFAULT ((0)) FOR [Status]
GO
ALTER TABLE [Invoice].[Terms] ADD  DEFAULT (getutcdate()) FOR [CreatedUtc]
GO
ALTER TABLE [Invoice].[Terms] ADD  DEFAULT (getutcdate()) FOR [ModifiedUtc]
GO
ALTER TABLE [Messaging].[CustomerTopics] ADD  DEFAULT (getutcdate()) FOR [CreatedUtc]
GO
ALTER TABLE [Messaging].[CustomerTopics] ADD  DEFAULT (getutcdate()) FOR [ModifiedUtc]
GO
ALTER TABLE [Messaging].[UserTopicSubscriptions] ADD  DEFAULT (getutcdate()) FOR [CreatedUtc]
GO
ALTER TABLE [Messaging].[UserTopicSubscriptions] ADD  DEFAULT (getutcdate()) FOR [ModifiedUtc]
GO
ALTER TABLE [Messaging].[UserTopicSubscriptions] ADD  DEFAULT ((0)) FOR [Channel]
GO
ALTER TABLE [Profile].[MarketingPreferences] ADD  DEFAULT (getutcdate()) FOR [CreatedUtc]
GO
ALTER TABLE [Profile].[MarketingPreferences] ADD  DEFAULT (getutcdate()) FOR [ModifiedUtc]
GO
ALTER TABLE [Invoice].[InvoiceItems]  WITH NOCHECK ADD  CONSTRAINT [FK_Invoice.InvoiceItems_Invoice.Invoices_InvoiceId] FOREIGN KEY([InvoiceId])
REFERENCES [Invoice].[Invoices] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [Invoice].[InvoiceItems] CHECK CONSTRAINT [FK_Invoice.InvoiceItems_Invoice.Invoices_InvoiceId]
GO
ALTER TABLE [List].[ListItems]  WITH CHECK ADD  CONSTRAINT [FK_List.ListItems_List.Lists_List_Id] FOREIGN KEY([ParentList_Id])
REFERENCES [List].[Lists] ([Id])
GO
ALTER TABLE [List].[ListItems] CHECK CONSTRAINT [FK_List.ListItems_List.Lists_List_Id]
GO
ALTER TABLE [List].[ListShares]  WITH CHECK ADD  CONSTRAINT [FK_List.ListShares_List.Lists_SharedList_Id] FOREIGN KEY([SharedList_Id])
REFERENCES [List].[Lists] ([Id])
GO
ALTER TABLE [List].[ListShares] CHECK CONSTRAINT [FK_List.ListShares_List.Lists_SharedList_Id]
GO
ALTER TABLE [Messaging].[UserTopicSubscriptions]  WITH CHECK ADD  CONSTRAINT [FK_Messaging.UserTopicSubscriptions_Messaging.CustomerTopics_CustomerTopic_Id] FOREIGN KEY([CustomerTopic_Id])
REFERENCES [Messaging].[CustomerTopics] ([Id])
GO
ALTER TABLE [Messaging].[UserTopicSubscriptions] CHECK CONSTRAINT [FK_Messaging.UserTopicSubscriptions_Messaging.CustomerTopics_CustomerTopic_Id]
GO
ALTER TABLE [Orders].[OrderHistoryDetail]  WITH CHECK ADD  CONSTRAINT [FK_Orders.OrderHistoryDetail_Orders.OrderHistoryHeader_OrderHistoryHeader_Id] FOREIGN KEY([OrderHistoryHeader_Id])
REFERENCES [Orders].[OrderHistoryHeader] ([Id])
GO
ALTER TABLE [Orders].[OrderHistoryDetail] CHECK CONSTRAINT [FK_Orders.OrderHistoryDetail_Orders.OrderHistoryHeader_OrderHistoryHeader_Id]
GO