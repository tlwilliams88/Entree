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
USE [BEK_Commerce_AppData]
GO
/****** Object:  StoredProcedure [ETL].[ReadBranches]    Script Date: 11/11/2014 2:10:33 PM ******/
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
/****** Object:  StoredProcedure [ETL].[ReadBrandControlLabels]    Script Date: 11/11/2014 2:10:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:        <Author,,Name>
-- Create date: <Create Date,,>
-- Description:    <Description,,>
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
/****** Object:  StoredProcedure [ETL].[ReadCustomers]    Script Date: 11/11/2014 2:10:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
 
 
 
 
-- =============================================
-- Author:        <Author,,Name>
-- Create date: <Create Date,,>
-- Description:    <Description,,>
-- =============================================
CREATE PROCEDURE [ETL].[ReadCustomers]
AS
BEGIN
   -- SET NOCOUNT ON added to prevent extra result sets from
   -- interfering with SELECT statements.
   SET NOCOUNT ON;
 
   SELECT 
       CO AS BranchNumber,
       CustomerNumber,
       CustomerName,
       Address1,
       Address2,
       City,
       [State],
       ZipCode,
       Telephone,
       SalesRep as DsrNumber,
       ChainStoreCode as NationalOrRegionalAccountNumber,
       [Contract] as ContractNumber,
       PORequiredFlag,
       PowerMenu,
       ContractOnly,
       TermCode,
       CreditLimit,
       CreditHoldFlag,
       DateOfLastPayment,
       AmountDue,
       CurrentBalance,
       PDACXAge1 BalanceAge1,
       PDACXAge2 BalanceAge2,
       PDACXAge3 BalanceAge3,
       PDACXAge4 BalanceAge4
   FROM 
       [ETL].Staging_Customer
 
END
 
 
 
GO
/****** Object:  StoredProcedure [ETL].[ReadFullItemData]    Script Date: 11/11/2014 2:10:33 PM ******/
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
/****** Object:  StoredProcedure [ETL].[ReadInvoices]    Script Date: 11/11/2014 2:10:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
 
-- =============================================
-- Author:        <Author,,Name>
-- Create date: <Create Date,,>
-- Description:    <Description,,>
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
/****** Object:  StoredProcedure [ETL].[ReadItemGS1Data]    Script Date: 11/11/2014 2:10:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:        <Author,,Name>
-- Create date: <Create Date,,>
-- Description:    <Description,,>
-- =============================================
Create PROCEDURE [ETL].[ReadItemGS1Data]
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
/****** Object:  StoredProcedure [ETL].[ReadItemsByBranch]    Script Date: 11/11/2014 2:10:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
 
-- =============================================
-- Author:        <Author,,Name>
-- Create date: <Create Date,,>
-- Description:    <Description,,>
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
/****** Object:  StoredProcedure [ETL].[ReadParentCategories]    Script Date: 11/11/2014 2:10:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
 
 
-- =============================================
-- Author:        <Author,,Name>
-- Create date: <Create Date,,>
-- Description:    <Description,,>
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
/****** Object:  StoredProcedure [ETL].[ReadProprietaryItems]    Script Date: 11/11/2014 2:10:33 PM ******/
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
/****** Object:  StoredProcedure [ETL].[ReadSubCategories]    Script Date: 11/11/2014 2:10:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
 
 
-- =============================================
-- Author:        <Author,,Name>
-- Create date: <Create Date,,>
-- Description:    <Description,,>
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
/****** Object:  StoredProcedure [ETL].[usp_ECOM_SelectContractItems]    Script Date: 11/11/2014 2:10:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
 
CREATE PROCEDURE [ETL].[usp_ECOM_SelectContractItems]
   @CustomerNumber varchar(10)
   , @DivisionName char(3)
   , @ContractNumber varchar(10)
 AS
 
/*******************************************************************
* PROCEDURE: usp_ECOM_SelectDistinctCustomerContracts
* PURPOSE: Select distinct customer contracts by division
* NOTES: {special set up or requirements, etc.}
* CREATED:    Jason McMillan 9/30/14
* MODIFIED 
* DATE        AUTHOR            DESCRIPTION
*-------------------------------------------------------------------
* {date}    {developer}    {brief modification description}
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
   LTRIM(RTRIM(bh.BidNumber)) = @ContractNumber
   AND LTRIM(RTRIM(c.CustomerNumber)) = @CustomerNumber
   AND LTRIM(RTRIM(cb.DivisionNumber)) = @DivisionName
ORDER BY
   bd.BidLineNumber ASC
 
 
/*
EXEC ETL.usp_ECOM_SelectContractItems '415101', 'FAM', 'D415101'
*/
 
 
 
GO
/****** Object:  StoredProcedure [ETL].[usp_ECOM_SelectCSUsers]    Script Date: 11/11/2014 2:10:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
 
CREATE PROCEDURE [ETL].[usp_ECOM_SelectCSUsers]
AS
 
/*******************************************************************
* PROCEDURE: usp_ECOM_SelectCSUsers
* PURPOSE: Select distinct commerce server users and email addresses
* NOTES: {special set up or requirements, etc.}
* CREATED:    Jason McMillan 2014-10-10
* MODIFIED 
* DATE        AUTHOR            DESCRIPTION
*-------------------------------------------------------------------
* {date}    {developer}    {brief modification description}
*******************************************************************/
 
SET NOCOUNT ON
 
SELECT
   DISTINCT
   u_user_id
FROM
   [BEK_Commerce_profiles]..[UserObject]
 
 
GO
/****** Object:  StoredProcedure [ETL].[usp_ECOM_SelectDistinctCustomerContracts]    Script Date: 11/11/2014 2:10:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
 
CREATE PROCEDURE [ETL].[usp_ECOM_SelectDistinctCustomerContracts] 
   @CustomerNumber varchar(10)
   , @DivisionName char(3)
   AS
 
/*******************************************************************
* PROCEDURE: usp_ECOM_SelectDistinctCustomerContracts
* PURPOSE: Select distinct customer contracts by division
* NOTES: {special set up or requirements, etc.}
* CREATED:    Jason McMillan 9/30/14
* MODIFIED 
* DATE        AUTHOR            DESCRIPTION
*-------------------------------------------------------------------
* {date}    {developer}    {brief modification description}
*******************************************************************/
 
SET NOCOUNT ON;
 
SELECT
   DISTINCT
   LTRIM(RTRIM(c.CustomerNumber)) 'CustomerNumber'
   , LTRIM(RTRIM(cb.BidNumber)) 'ContractNumber'
   , cb.DivisionNumber 'DivisionName'
FROM
   ETL.Staging_Customer c 
   INNER JOIN ETL.Staging_CustomerBid cb on c.CustomerNumber = cb.CustomerNumber
WHERE
   LTRIM(RTRIM(c.CustomerNumber)) = @CustomerNumber
   AND cb.DivisionNumber = @DivisionName
 
   
 
GO
/****** Object:  StoredProcedure [Orders].[GetNextControlNumber]    Script Date: 11/11/2014 2:10:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
 
CREATE PROCEDURE [Orders].[GetNextControlNumber]
   
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
/****** Object:  UserDefinedFunction [ETL].[initcap]    Script Date: 11/11/2014 2:10:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE function [ETL].[initcap] (@text varchar(4000))
returns varchar(4000)
as
 
begin
   declare     @counter int, 
       @length int,
       @char char(1),
       @textnew varchar(4000)
   if(@text = '')
   begin
       return @text
   end
 
   set @text        = rtrim(@text)
   set @text        = lower(@text)
   set @length     = len(@text)
   set @counter     = 1
 
   set @text = upper(left(@text, 1) ) + right(@text, @length - 1) 
 
   while @counter <> @length --+ 1
   begin
       select @char = substring(@text, @counter, 1)
 
       IF @char = space(1)  or @char =  '_' or @char = ','  or @char = '.' or @char = '\'
 or @char = '/' or @char = '(' or @char = ')'
       begin
           set @textnew = left(@text, @counter)  + upper(substring(@text, 
@counter+1, 1)) + right(@text, (@length - @counter) - 1)
           set @text     = @textnew
       end
 
       set @counter = @counter + 1
   end
 
   return @text
end
 
 
 
GO
/****** Object:  Table [ETL].[Staging_BidContractDetail]    Script Date: 11/11/2014 2:10:33 PM ******/
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
/****** Object:  Table [ETL].[Staging_BidContractHeader]    Script Date: 11/11/2014 2:10:33 PM ******/
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
/****** Object:  Table [ETL].[Staging_Branch]    Script Date: 11/11/2014 2:10:33 PM ******/
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
/****** Object:  Table [ETL].[Staging_BrandControlLabels]    Script Date: 11/11/2014 2:10:33 PM ******/
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
/****** Object:  Table [ETL].[Staging_Brands]    Script Date: 11/11/2014 2:10:33 PM ******/
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
/****** Object:  Table [ETL].[Staging_Category]    Script Date: 11/11/2014 2:10:33 PM ******/
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
/****** Object:  Table [ETL].[Staging_Customer]    Script Date: 11/11/2014 2:10:33 PM ******/
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
   [PowerMenu] [varchar](1) NULL
) ON [PRIMARY]
 
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [ETL].[Staging_CustomerBid]    Script Date: 11/11/2014 2:10:33 PM ******/
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
/****** Object:  Table [ETL].[Staging_FSE_ProductAllergens]    Script Date: 11/11/2014 2:10:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [ETL].[Staging_FSE_ProductAllergens](
   [Gtin] [char](14) NULL,
   [AllergenTypeCode] [varchar](2) NULL,
   [AllergenTypeDesc] [varchar](50) NULL,
   [LevelOfContainment] [varchar](20) NULL
) ON [PRIMARY]
 
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [ETL].[Staging_FSE_ProductDiet]    Script Date: 11/11/2014 2:10:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [ETL].[Staging_FSE_ProductDiet](
   [Gtin] [char](14) NULL,
   [DietType] [varchar](25) NULL,
   [Value] [char](1) NULL
) ON [PRIMARY]
 
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [ETL].[Staging_FSE_ProductNutrition]    Script Date: 11/11/2014 2:10:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
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
/****** Object:  Table [ETL].[Staging_FSE_ProductSpec]    Script Date: 11/11/2014 2:10:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
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
/****** Object:  Table [ETL].[Staging_ItemData]    Script Date: 11/11/2014 2:10:33 PM ******/
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
/****** Object:  Table [ETL].[Staging_KNet_Invoice]    Script Date: 11/11/2014 2:10:33 PM ******/
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
/****** Object:  Table [ETL].[Staging_OpenDetailAR]    Script Date: 11/11/2014 2:10:33 PM ******/
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
/****** Object:  Table [ETL].[Staging_PaidDetail]    Script Date: 11/11/2014 2:10:33 PM ******/
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
/****** Object:  Table [ETL].[Staging_ProprietaryCustomer]    Script Date: 11/11/2014 2:10:33 PM ******/
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
/****** Object:  Table [ETL].[Staging_ProprietaryItem]    Script Date: 11/11/2014 2:10:33 PM ******/
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
/****** Object:  Table [ETL].[Staging_Terms]    Script Date: 11/11/2014 2:10:33 PM ******/
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
/****** Object:  Table [ETL].[Staging_WorksheetItems]    Script Date: 11/11/2014 2:10:33 PM ******/
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
--CREATE PROCEDURE ETL.ReadAverageItemUsage       
--       @NumDays int
--AS
 
----NEED TO ADD SUMMARY COMMENTS HERE
 
--SET NOCOUNT ON;
 
--SELECT
--       oh.BranchId
--       , oh.CustomerNumber
--       , od.ItemNumber
--       , od.unitOfMeasure
--       , AVG(od.ShippedQuantity) 'AverageUse'
--FROM 
--       Orders.OrderHistoryHeader oh
--              INNER JOIN Orders.OrderHistoryDetail od ON od.OrderHistoryHeader_Id = oh.Id
--WHERE 
--       oh.CreatedUtc > DATEADD(DD, (@NumDays * -1), GETDATE())
--GROUP BY 
--       oh.BranchId
--       , oh.CustomerNumber
--       , od.ItemNumber
--       , od.unitOfMeasure
 
 
USE [BEK_Commerce_AppData]
GO
/****** Object:  StoredProcedure [ETL].[ProcessItemHistoryData]    Script Date: 6/17/2015 9:03:26 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
 
ALTER PROCEDURE [ETL].[ProcessItemHistoryData]       
       @NumWeeks int
AS
 
SET NOCOUNT ON;
 
TRUNCATE TABLE [Customers].[ItemHistory];
 
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
 
 
 