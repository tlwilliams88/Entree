USE BEK_Commerce_AppData
GO
DELETE [BEK_Commerce_AppData].[Configuration].[MessageTemplates] WHERE [TemplateKey] = 'ContractChangeNotice'
GO
DELETE [BEK_Commerce_AppData].[Configuration].[MessageTemplates] WHERE [TemplateKey] = 'ContractChangeItem'
GO
declare @Templates as TABLE 
( 
    [TemplateKey] NVARCHAR (50)  NULL,
    [Subject]     NVARCHAR (MAX) NULL,
    [IsBodyHtml]  BIT            NOT NULL,
    [Body]        NVARCHAR (MAX) NULL,
    [CreatedUtc]  DATETIME       DEFAULT (getutcdate()) NOT NULL,
    [ModifiedUtc] DATETIME       DEFAULT (getutcdate()) NOT NULL,
    [Type]        INT            DEFAULT ((0)) NOT NULL
)

INSERT INTO @Templates
VALUES ('NotifHeader', '', 1, '<table style="width: 100%;">
<tr>
<td>|LOGO|</td>
<td style="text-align:center;"><h3>{Subject}</h3></td>
<td>
<table>
<tr>
<td>{CustomerName}</td>
</tr>
<tr>
<td>Customer: {CustomerNumber}</td>
</tr>
<tr>
<td>Branch: {BranchID}</td>
</tr>
</table>
</td>
</tr>
</table>
<hr/>', getdate(), getdate(), 0)

MERGE INTO [BEK_Commerce_AppData].[Configuration].[MessageTemplates] A
USING @Templates B ON (A.[TemplateKey] = B.[TemplateKey])
WHEN MATCHED THEN
    UPDATE SET A.[Subject] = B.[Subject], A.[IsBodyHtml] = B.[IsBodyHtml], A.[Body] = B.[Body], A.[ModifiedUtc] = B.[ModifiedUtc], A.[Type] = B.[Type]
WHEN NOT MATCHED THEN
    INSERT ([TemplateKey],[Subject],[IsBodyHtml],[Body],[CreatedUtc],[ModifiedUtc],[Type]) 
	  VALUES(B.[TemplateKey],B.[Subject],B.[IsBodyHtml],B.[Body],B.[CreatedUtc],B.[ModifiedUtc],B.[Type]);