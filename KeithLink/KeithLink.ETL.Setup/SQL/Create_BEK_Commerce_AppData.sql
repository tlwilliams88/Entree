USE [master]
GO

/****** Object:  Database [BEK_Commerce_AppData]    Script Date: 7/21/2014 12:40:08 PM ******/
CREATE DATABASE [BEK_Commerce_AppData]
GO

ALTER DATABASE [BEK_Commerce_AppData] SET COMPATIBILITY_LEVEL = 110
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInacstalled'))
BEGIN
	EXEC [BEK_Commerce_AppData].[dbo].[sp_fulltext_database] @action = 'enable'
END 
GO

ALTER DATABASE [BEK_Commerce_AppData] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [BEK_Commerce_AppData] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [BEK_Commerce_AppData] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [BEK_Commerce_AppData] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [BEK_Commerce_AppData] SET ARITHABORT OFF 
GO
ALTER DATABASE [BEK_Commerce_AppData] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [BEK_Commerce_AppData] SET AUTO_CREATE_STATISTICS ON 
GO
ALTER DATABASE [BEK_Commerce_AppData] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [BEK_Commerce_AppData] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [BEK_Commerce_AppData] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [BEK_Commerce_AppData] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [BEK_Commerce_AppData] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [BEK_Commerce_AppData] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [BEK_Commerce_AppData] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [BEK_Commerce_AppData] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [BEK_Commerce_AppData] SET  DISABLE_BROKER 
GO
ALTER DATABASE [BEK_Commerce_AppData] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [BEK_Commerce_AppData] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [BEK_Commerce_AppData] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [BEK_Commerce_AppData] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [BEK_Commerce_AppData] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [BEK_Commerce_AppData] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [BEK_Commerce_AppData] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [BEK_Commerce_AppData] SET RECOVERY FULL 
GO
ALTER DATABASE [BEK_Commerce_AppData] SET  MULTI_USER 
GO
ALTER DATABASE [BEK_Commerce_AppData] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [BEK_Commerce_AppData] SET DB_CHAINING OFF 
GO
ALTER DATABASE [BEK_Commerce_AppData] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [BEK_Commerce_AppData] SET TARGET_RECOVERY_TIME = 0 SECONDS 
GO
EXEC sys.sp_db_vardecimal_storage_format N'BEK_Commerce_AppData', N'ON'
GO
USE [BEK_Commerce_AppData]
GO

/****** Object:  Schema [ETL]    Script Date: 7/21/2014 12:40:09 PM ******/
CREATE SCHEMA [ETL]
GO
/****** Object:  UserDefinedFunction [ETL].[initcap]    Script Date: 7/21/2014 12:40:09 PM ******/
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
/****** Object:  Table [ETL].[Staging_Branch]    Script Date: 7/21/2014 12:40:09 PM ******/
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
/****** Object:  Table [ETL].[Staging_Category]    Script Date: 7/21/2014 12:40:09 PM ******/
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
/****** Object:  Table [ETL].[Staging_Customer]    Script Date: 7/21/2014 12:40:09 PM ******/
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
	[DateOfLastPayment] [date] NULL,
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
	[PowerMenu] [varchar](1) NULL
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [ETL].[Staging_ItemData]    Script Date: 7/21/2014 12:40:09 PM ******/
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
/****** Object:  Table [ETL].[Staging_Category]    Script Date: 7/21/2014 12:40:09 PM ******/
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
/****** Object:  Table [ETL].[Staging_Category]    Script Date: 7/21/2014 12:40:09 PM ******/
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
/********************************************************************************************/


SET ANSI_NULLS ON

SET QUOTED_IDENTIFIER ON

SET ANSI_PADDING ON

IF (NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES 
					WHERE TABLE_SCHEMA = 'dbo' 
					AND  TABLE_NAME = 'Log'))
BEGIN

	CREATE TABLE [dbo].[Log](
		[Id] [int] IDENTITY(1,1) NOT NULL,
		[Date] [datetime] NOT NULL,
		[Thread] [varchar](255) NOT NULL,
		[Host] [varchar] (255) NOT NULL,
		[User] [varchar] (255) NOT NULL,
		[Application] [varchar] (255) NOT NULL,
		[Level] [varchar](50) NOT NULL,
		[Logger] [varchar](255) NOT NULL,
		[Message] [varchar](MAX) NOT NULL,
		[Exception] [varchar](MAX) NULL
	) ON [PRIMARY]

	SET ANSI_PADDING OFF

	CREATE UNIQUE CLUSTERED INDEX PK_Log ON [Log] 
		([Id]) 

	CREATE INDEX IX_Log_Date
		ON [dbo].[Log] ([Date]);

END

/*** Add GS1 Tables **/
USE BEK_Commerce_AppData;

CREATE TABLE ETL.Staging_FSE_ProductSpec (
	Gtin char(14) 
	, PackageGtin char(14)
	, BekItemNumber char(6)
	, Active bit
	, ProductType varchar(20)
	, ProductShortDesc varchar(70)
	, ProductAdditionalDesc varchar(max)
	, ManufacturerItemNumber varchar(30)
	, UnitsPerCase int
	, UnitMeasure decimal(20,3)
	, UnitMeasureUOM varchar(3)
	, GrossWeight decimal(20,3)
	, NetWeight decimal(20,3)
	, Length decimal(20,3)
	, Width decimal(20,3)
	, Height decimal(20,3)
	, Volume decimal(20,3)
	, TiHi varchar(250)
	, Shelf int
	, StorageTemp varchar(35)
	, ServingsPerPack int
	, ServingSuggestion varchar(max)
	, MoreInformation varchar(35)
	, MarketingMessage varchar(max)
	, ServingSize decimal(20,3)
	, ServingSizeUOM varchar(3)
	, Ingredients varchar(max)
	, Brand varchar(35)
	, BrandOwner varchar(max)
	, CountryOfOrigin varchar(100)
	, ContryOfOriginName varchar(100)
	, PreparationInstructions varchar(max)
	, HandlingInstruction varchar(max)
);
GO

CREATE TABLE ETL.Staging_FSE_ProductNutrition (
	Gtin char(14) 
	, NutrientTypeCode varchar(100)
	, NutrientTypeDesc varchar(150)
	, MeasurmentTypeId varchar(5)
	, MeasurementValue decimal(20,3)
	, DailyValue varchar(100)
);
GO

CREATE TABLE ETL.Staging_FSE_ProductDiet (
	Gtin char(14)
	, DietType varchar(25)
	, Value char(1)
);

GO

CREATE TABLE ETL.Staging_FSE_ProductAllergens (
	Gtin char(14)
	, AllergenTypeCode varchar(2)
	, AllergenTypeDesc varchar(50)
	, LevelOfContainment varchar(20)
);
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

CREATE TABLE [ETL].[Staging_ProprietaryCustomer](
	[Action] [varchar](1) NULL,
	[CompanyNumber] [varchar](3) NULL,
	[DivisionNumber] [varchar](3) NULL,
	[DepartmentNumber] [varchar](3) NULL,
	[ProprietaryNumber] [varchar](10) NULL,
	[CustomerNumber] [varchar](10) NULL
) ON [PRIMARY]

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
/****** Object:  StoredProcedure [ETL].[ReadFullItemData]    Script Date: 8/12/2014 2:56:51 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [ETL].[ReadFullItemData]
	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	SELECT 
		BranchId, 
		ItemId, 
		ETL.initcap(Name) as Name, 
		ETL.initcap(Description) as Description, 
		i.Brand, 
		Pack, 
		Size, 
		UPC, 
		MfrNumber, 
		MfrName, 
		(SELECT b.ExtendedDescription FROM ETL.Staging_Brands b WHERE b.Brand = i.Brand) As BrandDescription,
		MaxSmrt,
		Cases, 
		Package, 
		PreferredItemCode, 
		ItemType, 
		Status1, 
		Status2, 
		ICSEOnly, 
		SpecialOrderItem, 
		Vendor1, 
		Vendor2, 
		Class, 
		CatMgr, 
		HowPrice, 
		Buyer, 
		Kosher, 
		c.CategoryId, 
		ReplacementItem, 
		ReplacedItem, 
		CNDoc,
		[Cube], 
		ETL.initcap(c.CategoryName) as CategoryName, 
		(SELECT CategoryId from ETL.Staging_Category WHERE CategoryId = SUBSTRING(c.CategoryId, 1, 2) + '000') as ParentCategoryId, 
		(SELECT ETL.initcap(CategoryName) from ETL.Staging_Category WHERE CategoryId = SUBSTRING(c.CategoryId, 1, 2) + '000') as ParentCategoryName,
		ps.BrandOwner,
		ps.CountryOfOrigin,
		ps.GrossWeight,
		ps.HandlingInstruction,
		ps.Height,
		ps.Ingredients,
		ps.Length,
		ps.MarketingMessage,
		ps.MoreInformation,
		ps.ServingSize,
		ps.ServingSizeUOM,
		ps.ServingsPerPack,
		ps.ServingSuggestion,
		ps.Shelf,
		ps.StorageTemp,
		ps.UnitMeasure,
		ps.UnitsPerCase,
		ps.Volume,
		ps.Width,
		i.NonStock,
		i.FDAProductFlag,
		i.TempZone	 
	FROM  
		ETL.Staging_ItemData i inner join 
		ETL.Staging_Category c on i.CategoryId = c.CategoryId left outer join
		ETL.Staging_FSE_ProductSpec ps on i.UPC = ps.Gtin
	WHERE 
		  i.ItemId NOT LIKE '999%'  AND SpecialOrderItem <>'Y'
END

GO
/****** Object:  StoredProcedure [ETL].[ReadItemGS1Data]    Script Date: 8/12/2014 2:56:51 PM ******/
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
/****** Object:  StoredProcedure [ETL].[ReadItemsByBranch]    Script Date: 8/12/2014 2:56:51 PM ******/
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
/****** Object:  StoredProcedure [ETL].[ReadParentCategories]    Script Date: 8/12/2014 2:56:51 PM ******/
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
/****** Object:  StoredProcedure [ETL].[ReadSubCategories]    Script Date: 8/12/2014 2:56:51 PM ******/
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

/****** Object:  StoredProcedure [ETL].[ReadItemGS1Data]    Script Date: 8/12/2014 2:56:51 PM ******/
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
	SELECT DISTINCT
		b.ControlLabel,
		b.ExtendedDescription
	FROM
		ETL.Staging_BrandControlLabels b
END

GO
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


SET ANSI_PADDING OFF
GO
USE [master]
GO
ALTER DATABASE [BEK_Commerce_AppData] SET  READ_WRITE 
GO
