INSERT 
	INTO [Configuration].[AppSettings]
		([Key], [Value], [Comment], [Disabled]) 
	VALUES 
		('DiagnosticsAuditOrderHistoryHeaderChanges', 'false', 'Diagnostics', 0)

INSERT 
	INTO [Configuration].[AppSettings]
		([Key], [Value], [Comment], [Disabled]) 
	VALUES 
		('DiagnosticsAuditOrderHistoryDetailChanges', 'false', 'Diagnostics', 0)

INSERT 
	INTO [Configuration].[AppSettings]
		([Key], [Value], [Comment], [Disabled]) 
	VALUES 
		('OrderHistoryPersistenceAttemptLimit', '10', 'Order History processing options', 0)

INSERT 
	INTO [Configuration].[AppSettings]
		([Key], [Value], [Comment], [Disabled]) 
	VALUES 
		('OrderHistoryPersistenceAttemptInterval', '.5', 'Order History processing options', 0)