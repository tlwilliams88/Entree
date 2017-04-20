
-- Queue Service Functions
INSERT @AppSettings ([Key], [Value], [Comment], [Disabled]) VALUES ('ProcessContractChanges', 'true', 'Queue Service Functions', 0)
INSERT @AppSettings ([Key], [Value], [Comment], [Disabled]) VALUES ('ContractChangesPurgeDays', '-7', 'Misc Settings', 0)
