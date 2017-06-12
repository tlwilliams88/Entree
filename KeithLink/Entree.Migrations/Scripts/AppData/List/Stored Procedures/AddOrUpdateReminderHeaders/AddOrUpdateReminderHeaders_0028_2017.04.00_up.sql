CREATE PROCEDURE [List].[AddOrUpdateRemindersHeaders] 
    @Id             BIGINT,
    @BranchId       CHAR(3),
    @CustomerNumber CHAR(6)
AS
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

IF @Id > 0
    UPDATE
        [List].[RemindersHeaders]
    SET
        [BranchId] = @BranchId,
        [CustomerNumber] = @CustomerNumber,
        [ModifiedUtc] = GETUTCDATE()
    WHERE
        Id = @Id

ELSE
      BEGIN
        INSERT INTO
            [List].[RemindersHeaders] (
                [BranchId],
                [CustomerNumber],
                [CreatedUtc],
                [ModifiedUtc]
            ) VALUES (
                @BranchId,
                @CustomerNumber,
                GETUTCDATE(),
                GETUTCDATE()
            )
      END