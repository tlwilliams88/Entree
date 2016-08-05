CREATE PROCEDURE [Configuration].[SaveAppSetting]
	@Key		VARCHAR(50),
	@Value		VARCHAR(MAX),
	@Comment	VARCHAR(MAX),
	@Disabled	BIT
AS
	UPDATE	Configuration.AppSettings
	SET
		[Value] = @Value,
		[Comment] = @Comment,
		[Disabled] = @Disabled
	WHERE
		[Key] = @Key