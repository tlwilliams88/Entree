USE [master]
GO
/****** Object:  Database [KeithLinkAppData]    Script Date: 7/21/2014 12:40:08 PM ******/
CREATE DATABASE [KeithLinkAppData]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'KeithLinkAppData', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL11.MSSQLSERVER\MSSQL\DATA\KeithLinkAppData.mdf' , SIZE = 83968KB , MAXSIZE = UNLIMITED, FILEGROWTH = 1024KB )
 LOG ON 
( NAME = N'KeithLinkAppData_log', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL11.MSSQLSERVER\MSSQL\DATA\KeithLinkAppData_log.ldf' , SIZE = 517184KB , MAXSIZE = 2048GB , FILEGROWTH = 10%)
GO
ALTER DATABASE [KeithLinkAppData] SET COMPATIBILITY_LEVEL = 110
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [KeithLinkAppData].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [KeithLinkAppData] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [KeithLinkAppData] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [KeithLinkAppData] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [KeithLinkAppData] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [KeithLinkAppData] SET ARITHABORT OFF 
GO
ALTER DATABASE [KeithLinkAppData] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [KeithLinkAppData] SET AUTO_CREATE_STATISTICS ON 
GO
ALTER DATABASE [KeithLinkAppData] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [KeithLinkAppData] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [KeithLinkAppData] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [KeithLinkAppData] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [KeithLinkAppData] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [KeithLinkAppData] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [KeithLinkAppData] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [KeithLinkAppData] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [KeithLinkAppData] SET  DISABLE_BROKER 
GO
ALTER DATABASE [KeithLinkAppData] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [KeithLinkAppData] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [KeithLinkAppData] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [KeithLinkAppData] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [KeithLinkAppData] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [KeithLinkAppData] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [KeithLinkAppData] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [KeithLinkAppData] SET RECOVERY FULL 
GO
ALTER DATABASE [KeithLinkAppData] SET  MULTI_USER 
GO
ALTER DATABASE [KeithLinkAppData] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [KeithLinkAppData] SET DB_CHAINING OFF 
GO
ALTER DATABASE [KeithLinkAppData] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [KeithLinkAppData] SET TARGET_RECOVERY_TIME = 0 SECONDS 
GO
EXEC sys.sp_db_vardecimal_storage_format N'KeithLinkAppData', N'ON'
GO
USE [KeithLinkAppData]
GO
/****** Object:  User [WIN2012BEK\Administrator]    Script Date: 7/21/2014 12:40:08 PM ******/
CREATE USER [WIN2012BEK\Administrator] FOR LOGIN [WIN2012BEK\Administrator] WITH DEFAULT_SCHEMA=[dbo]
GO
ALTER ROLE [db_owner] ADD MEMBER [WIN2012BEK\Administrator]
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
	[HACCPDoce] [char](5) NULL
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
USE [master]
GO
ALTER DATABASE [KeithLinkAppData] SET  READ_WRITE 
GO
