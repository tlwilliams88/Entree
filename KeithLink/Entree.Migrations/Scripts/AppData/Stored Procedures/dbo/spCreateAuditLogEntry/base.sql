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
