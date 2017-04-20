
MERGE INTO [BEK_Commerce_AppData].[Configuration].[MessageTemplates] A
USING @Templates B ON (A.[TemplateKey] = B.[TemplateKey])
WHEN MATCHED THEN
    UPDATE SET A.[Subject] = B.[Subject], A.[IsBodyHtml] = B.[IsBodyHtml], A.[Body] = B.[Body], A.[ModifiedUtc] = B.[ModifiedUtc], A.[Type] = B.[Type]
WHEN NOT MATCHED THEN
    INSERT ([TemplateKey],[Subject],[IsBodyHtml],[Body],[CreatedUtc],[ModifiedUtc],[Type]) 
	  VALUES(B.[TemplateKey],B.[Subject],B.[IsBodyHtml],B.[Body],B.[CreatedUtc],B.[ModifiedUtc],B.[Type]);