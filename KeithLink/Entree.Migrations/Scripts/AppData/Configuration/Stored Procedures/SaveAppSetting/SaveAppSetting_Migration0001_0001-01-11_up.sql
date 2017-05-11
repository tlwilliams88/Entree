/****** Object:  StoredProcedure [Configuration].[SaveAppSetting]    Script Date: 10/27/2016 1:05:24 PM ******/
SET ANSI_NULLS OFF
GO
SET QUOTED_IDENTIFIER OFF
GO
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

GO
