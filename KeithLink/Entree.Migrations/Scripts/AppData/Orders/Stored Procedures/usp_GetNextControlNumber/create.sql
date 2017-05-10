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
