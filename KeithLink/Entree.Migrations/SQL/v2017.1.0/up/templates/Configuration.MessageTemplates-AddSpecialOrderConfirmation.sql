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
VALUES ('SpecialOrderConfirmation', 'Ben E. Keith: {OrderStatus} for {CustomerNumber}-{CustomerName}', 1, 
'{NotifHeader}<table style="width: 100%;">
   <tr>
       <td>Warehouse Delivery Date: {ShipDate}</td>
   </tr>
   <tr>
       <td>Sent with Invoice Number: {InvoiceNumber}</td>
       <td style="text-align:right;">Item Price: ${Total}</td>
   </tr>
</table>
<table style="width: 100%;">
	<tr style="border-bottom:1px solid gray;">
		<th style="text-align:left;">Item # </th>
		<th style="text-align:left;">GTIN </th>
		<th style="text-align:left;">Source </th>
		<th style="text-align:left;">Confirmed Items </th>
		<th style="text-align:right;">Quantity</th>
	</tr>
	<tr style="border-bottom:1px solid gray;">
		<td style="text-align:left;">{ItemNumber}</td>
		<td style="text-align:left;">{GTIN}</td>
		<td style="text-align:left;">{Source}</td>
		<td style="text-align:left;">{Description}</td>
		<td style="text-align:right;">{Quantity}</td>
	</tr>
</table>', getdate(), getdate(), 0)

MERGE INTO [BEK_Commerce_AppData].[Configuration].[MessageTemplates] A
USING @Templates B ON (A.[TemplateKey] = B.[TemplateKey])
WHEN MATCHED THEN
    UPDATE SET A.[Subject] = B.[Subject], A.[IsBodyHtml] = B.[IsBodyHtml], A.[Body] = B.[Body], A.[ModifiedUtc] = B.[ModifiedUtc], A.[Type] = B.[Type]
WHEN NOT MATCHED THEN
    INSERT ([TemplateKey],[Subject],[IsBodyHtml],[Body],[CreatedUtc],[ModifiedUtc],[Type]) 
	  VALUES(B.[TemplateKey],B.[Subject],B.[IsBodyHtml],B.[Body],B.[CreatedUtc],B.[ModifiedUtc],B.[Type]);