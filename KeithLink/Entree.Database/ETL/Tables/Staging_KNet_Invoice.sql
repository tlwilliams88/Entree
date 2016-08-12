﻿CREATE TABLE [ETL].[Staging_KNet_Invoice] (
    [Action]                VARCHAR (1)  NULL,
    [CompanyNumber]         VARCHAR (3)  NULL,
    [DivisionNumber]        VARCHAR (3)  NULL,
    [DepartmentNumber]      VARCHAR (3)  NULL,
    [CustomerNumber]        VARCHAR (10) NULL,
    [OrderNumber]           VARCHAR (9)  NULL,
    [LineNumber]            VARCHAR (5)  NULL,
    [MemoBillCode]          VARCHAR (3)  NULL,
    [CreditOFlag]           VARCHAR (1)  NULL,
    [TradeSWFlag]           VARCHAR (1)  NULL,
    [ShipDate]              VARCHAR (8)  NULL,
    [OrderDate]             VARCHAR (8)  NULL,
    [RouteNumber]           VARCHAR (5)  NULL,
    [StopNumber]            VARCHAR (3)  NULL,
    [WHNumber]              VARCHAR (3)  NULL,
    [ItemNumber]            VARCHAR (10) NULL,
    [QuantityOrdered]       VARCHAR (7)  NULL,
    [QuantityShipped]       VARCHAR (7)  NULL,
    [BrokenCaseCode]        VARCHAR (1)  NULL,
    [CatchWeightCode]       VARCHAR (1)  NULL,
    [ExtCatchWeight]        VARCHAR (12) NULL,
    [ItemPrice]             VARCHAR (10) NULL,
    [PriceBookNumber]       VARCHAR (5)  NULL,
    [ItemPriceSRP]          VARCHAR (12) NULL,
    [OriginalInvoiceNumber] VARCHAR (20) NULL,
    [InvoiceNumber]         VARCHAR (20) NULL,
    [AC]                    VARCHAR (1)  NULL,
    [ChangeDate]            VARCHAR (8)  NULL,
    [DateOfLastOrder]       VARCHAR (8)  NULL,
    [ExtSRPAmount]          VARCHAR (12) NULL,
    [ExtSalesGross]         VARCHAR (16) NULL,
    [ExtSalesNet]           VARCHAR (16) NULL,
    [CustomerGroup]         VARCHAR (10) NULL,
    [SalesRep]              VARCHAR (3)  NULL,
    [VendorNumber]          VARCHAR (10) NULL,
    [CustomerPO]            VARCHAR (20) NULL,
    [ChainStoreCode]        VARCHAR (10) NULL,
    [CombStatementCustomer] VARCHAR (10) NULL,
    [PriceBook]             VARCHAR (7)  NULL,
    [ClassCode]             VARCHAR (5)  NULL
);
