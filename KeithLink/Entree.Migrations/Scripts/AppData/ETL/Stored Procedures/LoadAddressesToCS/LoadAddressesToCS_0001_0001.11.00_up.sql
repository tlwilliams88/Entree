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
