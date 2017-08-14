UPDATE [BranchSupport].[BranchSupports]
   SET [BranchName] = (
        case when Id = '2' then 'Amarillo'
        when Id = '3' then 'Dallas/Fort Worth'
        when Id = '4' then 'Houston'
        when Id = '5' then 'Little Rock'
        when Id = '6' then 'Arkansas'
        when Id = '7' then 'New Mexico'
        when Id = '8' then 'Oklahoma'
        when Id = '9' then 'San Antonio'
        when Id = '10' then 'Elba'
        end)
GO