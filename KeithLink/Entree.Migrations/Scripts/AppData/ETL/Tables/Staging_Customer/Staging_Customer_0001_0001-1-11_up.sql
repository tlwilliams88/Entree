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
