USE [BEK_Commerce_AppData]
GO

UPDATE [BranchSupport].[BranchSupports]
   SET [BranchName] = (
        case when Id = '2' then 'West Texas'
        when Id = '3' then 'Dallas/Fort Worth'
        when Id = '4' then 'Gulf Coast'
        when Id = '5' then 'Mid-South'
        when Id = '6' then 'Arkansas'
        when Id = '7' then 'New Mexico'
        when Id = '8' then 'Oklahoma'
        when Id = '9' then 'San Antonio'
        when Id = '10' then 'Southeast'
        end)
GO