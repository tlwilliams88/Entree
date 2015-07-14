------------------------------------------------------------------------------
-- Cleanup ETL Staging Tables 
--
-- Remove unused fields and tables from the ETL Staging data tables which are 
-- no longer used in the program.
--
-- Pivotal User Story:  95632712
-- Created: 7/13/2015
-- Last Modified 7/13/2015
-------------------------------------------------------------------------------

USE Bek_Commerce_Appdata;
GO

ALTER TABLE ETL.Staging_BidContractDetail
DROP COLUMN [Action];
GO

ALTER TABLE ETL.Staging_BidContractHeader
DROP COLUMN [Action];
GO

ALTER TABLE ETL.Staging_Customer
DROP COLUMN [Action],
			StoreNumber,
			LocationAuth,
			ActiveFlag,
			MondayRoutNum,
			MondayStopNum,
			TuesdayRoutNum,
			TuesdayStopNum,
			WednesdayRoutNum,
			WednesdayStopNum,
			ThursdayRoutNum,
			ThursdayStopNum,
			FridayRoutNum,
			FridayStopNum,
			SaturdayRoutNum,
			SaturdayStopNum,
			SundayRoutNum,
			SundayStopNum,
			CSR,
			PORequiredFlag,
			JDCNTY1,
			JDCNTY2,
			PowerMenu;
GO

ALTER TABLE ETL.Staging_CustomerBid
DROP COLUMN [Action],
			PriorityNumber;
GO

ALTER TABLE ETL.Staging_EmployeeInfo
DROP COLUMN EMPLID, 
			LAST_NAME, 
			FIRST_NAME;
GO

ALTER TABLE ETL.Staging_FSE_ProductSpec
DROP COLUMN PackageGtin,
			BekItemNumber,
			Active,
			ProductType,
			ProductShortDesc,
			ProductAdditionalDesc,
			ManufacturerItemNumber,
			UnitMeasureUOM,
			NetWeight,
			TiHi,
			Brand,
			CountryOfOriginName,
			PreparationInstructions;
GO

ALTER TABLE ETL.Staging_ItemData
DROP COLUMN PVTLbl,
			OrderTiHi,
			TiHi,
			DateDiscontinued,
			Dtelstal,
			DTELstPO,
			NetWeight,
			ShelfLife,
			DateSensitiveType,
			Country,
			[Length],
			Width,
			Height,
			MinTemp,
			MaxTemp,
			GDSNSync,
			GuaranteedDays,
			MasterPack,
			HACCP,
			HACCPDoce,
			FPLength,
			FPWidth,
			FPHeight,
			FPGrossWt,
			FPCube;
GO

ALTER TABLE ETL.Staging_KNet_Invoice
DROP Column [Action],
			MemoBillCode,
			CreditOFlag,
			TradeSWFlag,
			RouteNumber,
			StopNumber,
			WHNumber,
			BrokenCaseCode,
			PriceBookNumber,
			ItemPriceSRP,
			OriginalInvoiceNumber,
			AC,
			ChangeDate,
			DateOfLastOrder,
			ExtSRPAmount,
			ExtSalesGross,
			CustomerGroup,
			SalesRep,
			VendorNumber,
			CustomerPO,
			ChainStoreCode,
			CombStatementCustomer,
			PriceBook;
GO

DROP TABLE ETL.Staging_KPay_Invoice;
GO

ALTER TABLE ETL.Staging_OpenDetailAR
DROP COLUMN [Action],
			Division,
			Department,
			AC,
			ChangeDate,
			ReclineNumber,
			InvoiceType,
			DateOfLastOrder,
			InvoiceAmount,
			InvoiceDue,
			CombinedStmt;
GO

ALTER TABLE ETL.Staging_PaidDetail
DROP COLUMN [Action],
			AdjustCode,
			Company,
			Division,
			Department,
			ReclineNumber,
			InvoiceType,
			CheckNumber,
			InvoiceReference,
			CombinedStmt;
GO

ALTER TABLE ETL.Staging_ProprietaryCustomer
DROP COLUMN [Action],
			CompanyNumber,
			DivisionNumber,
			DepartmentNumber;
GO

ALTER TABLE ETL.Staging_ProprietaryCustomer
DROP COLUMN CompanyNumber,
			DivisionNumber,
			DepartmentNumber;
GO

ALTER TABLE ETL.Staging_Terms
DROP COLUMN [Action],
			Company,
			[Description],
			Prox;
GO

ALTER TABLE ETL.Staging_WorksheetItems
DROP Column [Action];
GO
		





