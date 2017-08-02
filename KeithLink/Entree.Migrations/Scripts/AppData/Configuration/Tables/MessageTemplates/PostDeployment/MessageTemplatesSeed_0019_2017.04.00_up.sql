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
<td style="text-align:right;">
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
INSERT INTO @Templates
VALUES ('ContractChangeNotice', 'Ben E. Keith: Contract Change Notice for {CustomerNumber}-{CustomerName}', 1, 
'{NotifHeader}<table style="width: 100%;">
	<tr style="border-bottom:1px solid gray;">
		<th style="text-align:left;">Change </th>
		<th style="text-align:left;">Item # </th>
		<th style="text-align:left;">Description </th>
		<th style="text-align:left;">Brand </th>
		<th style="text-align:left;">Pack </th>
		<th style="text-align:left;">Size </th>
	</tr>
{ContractChangeItems}
</table>', getdate(), getdate(), 0)
INSERT INTO @Templates
VALUES ('ContractChangeItem', '', 1, '<tr>
        <td style="text-align:left;font-size:small;">{Status} </td>
        <td style="text-align:left;font-size:small;">{ProductNumber} </td>
        <td style="text-align:left;font-size:small;">{ProductDescription} </td>
        <td style="text-align:left;font-size:small;">{Brand} </td>
        <td style="text-align:left;font-size:small;">{Pack} </td>
        <td style="text-align:left;font-size:small;">{Size} </td>
    </tr>', getdate(), getdate(), 0)

MERGE INTO [Configuration].[MessageTemplates] A
USING @Templates B ON (A.[TemplateKey] = B.[TemplateKey])
WHEN MATCHED THEN
    UPDATE SET A.[Subject] = B.[Subject], A.[IsBodyHtml] = B.[IsBodyHtml], A.[Body] = B.[Body], A.[ModifiedUtc] = B.[ModifiedUtc], A.[Type] = B.[Type]
WHEN NOT MATCHED THEN
    INSERT ([TemplateKey],[Subject],[IsBodyHtml],[Body],[CreatedUtc],[ModifiedUtc],[Type]) 
	  VALUES(B.[TemplateKey],B.[Subject],B.[IsBodyHtml],B.[Body],B.[CreatedUtc],B.[ModifiedUtc],B.[Type]);