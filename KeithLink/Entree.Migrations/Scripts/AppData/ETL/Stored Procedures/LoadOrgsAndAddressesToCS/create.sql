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
