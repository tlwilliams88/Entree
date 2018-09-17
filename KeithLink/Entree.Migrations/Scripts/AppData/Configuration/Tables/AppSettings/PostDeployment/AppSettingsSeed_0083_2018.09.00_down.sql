DELETE FROM [Configuration].AppSettings
WHERE [Key] = 'DiagnosticsAuditOrderHistoryHeaderChanges'

DELETE FROM [Configuration].AppSettings
WHERE [Key] = 'DiagnosticsAuditOrderHistoryDetailChanges'

DELETE FROM [Configuration].AppSettings
WHERE [Key] = 'OrderHistoryPersistenceAttemptLimit'

DELETE FROM [Configuration].AppSettings
WHERE [Key] = 'OrderHistoryPersistenceAttemptInterval'

