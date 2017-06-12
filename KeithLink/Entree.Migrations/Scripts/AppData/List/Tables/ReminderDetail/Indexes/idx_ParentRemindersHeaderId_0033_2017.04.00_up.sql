CREATE INDEX idx_ParentRemindersHeaderListId
    ON [List].[RemindersDetails] (
        [ParentRemindersHeaderId]
    )
GO