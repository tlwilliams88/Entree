CREATE INDEX idx_ParentRemindersHeaderListId
    ON [List].[ReminderDetails] (
        [ParentRemindersHeaderId]
    )
GO