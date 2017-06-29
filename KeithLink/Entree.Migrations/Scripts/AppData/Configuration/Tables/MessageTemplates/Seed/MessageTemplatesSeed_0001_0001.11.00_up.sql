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
VALUES ('OrderEtaMain', 'Ben E. Keith: Estimated Delivery Information for {CustomerNumber}-{CustomerName}', 1, '{NotifHeader}Order #: {InvoiceNumber} {ETAMessage}.
This order contains {ProductCount} items.<hr/>
<table style="width: 100%">
<tr>
<th style="font-size:small;text-align:left;">Item # </th>
<th style="font-size:small;text-align:left;">Item Description </th>
<th colspan="2" style="font-size:small;text-align:left;">Quantity</th>
</tr>
{OrderEtaLines}
</table>', getdate(), getdate(), 0)
INSERT INTO @Templates
VALUES ('OrderEtaLine', '', 1, '<tr>
<td style="font-size:small;text-align:left;">{ProductNumber} </td>
<td style="font-size:small;text-align:left;">{ProductDescription} </td>
<td colspan="2" style="font-size:small;text-align:left;">{Quantity}</td></tr>', getdate(), getdate(), 0)
INSERT INTO @Templates
VALUES ('PaymentConfirmation', 'Ben E. Keith: Payment Confirmation for {CustomerNumber}-{CustomerName}', 1, '{NotifHeader}
Bank: {BankAccount}<br/>
Confirmation: {ConfirmationId}<br/>
<table style="width: 100%">
<tr>
<th style="font-size:small;text-align:left;">Type </th>
<th style="font-size:small;text-align:left;">Number </th>
<th style="font-size:small;text-align:left;">Ref. Date </th>
<th style="font-size:small;text-align:left;">Due Date </th>
<th style="font-size:small;text-align:left;">Scheduled Date </th>
<th style="font-size:small;text-align:right;">Amount</th>
</tr>
{PaymentDetailLines}
<tr>
<td colspan="5" style="text-align:right">Customer Total</td>
<td style="text-align:right">
${TotalPayments:f2}
<td>
</tr>
</table>', getdate(), getdate(), 0)
INSERT INTO @Templates
VALUES ('PaymentConfirmationDetail', '', 1, '<tr>
<td style="font-size:small;text-align:left;">{InvoiceType} </td>
<td style="font-size:small;text-align:left;">{InvoiceNumber} </td>
<td style="font-size:small;text-align:left;">{InvoiceDate:MM/dd/yyyy} </td>
<td style="font-size:small;text-align:left;">{DueDate:MM/dd/yyyy} </td>
<td style="font-size:small;text-align:left;">{ScheduledDate:MM/dd/yyyy} </td>
<td style="font-size:small;text-align:right;">${PaymentAmount:f2}</td>
</tr>', getdate(), getdate(), 0)
INSERT INTO @Templates
VALUES ('MultiPaymentConfirmation', 'Payment Confirmation', 1, '
<html><head>
<meta http-equiv="Content-Type" content="text/html; charset=us-ascii"><style type="text/css" rel="stylesheet">
body { background-color: #FFFEF2; margin: 0; padding: 0; color: #353334; font-size: 10pt; font-family: Verdana, Tahoma, Calibri, sans-serif; }
TR.tableHead { COLOR: white; BACKGROUND-COLOR: #0054a3 }
TR.dataRow { background-color: white; }
TR.altDataRow { background-color: #e8e5cb; }
TR.tableFoot { background-color: #e6f3ff; }
.heading { FONT-WEIGHT: bold; FONT-SIZE: 14pt; FONT-FAMILY: Verdana, Tahoma, Calibri, sans-serif; color: #353334; }
.appLink { color: blue; text-decoration: underline; }
</style></head>
<body>
<table style="width: 100%;">
<tr>
<td>|LOGO|</td>
<td style="text-align:center;"><h3>Thank you for your payment.</h3></td>
<td style="text-align:center;">
Below is a list of the payments paid by {UserName}. The confirmation number for these payments are {ConfirmationId}.</td>
</tr>
</table>
<hr/>
{PaymentCollection}
</body></html>', getdate(), getdate(), 0)
INSERT INTO @Templates
VALUES ('MultiPaymentConfirmationCustomerHeader', '', 1, '<p style="FONT-WEIGHT: bold;">{BankName} - {AccountNumber}</p>
<table cellpadding="3" cellspacing="0" border="0" width="700">
<tr class="tableHead">
<td>Customer </td>
<td>Ref </td>
<td>Number </td>
<td align="right">Ref Date </td>
<td align="right">Due Date </td>
<td align="right">Amount </td>
</tr>', getdate(), getdate(), 0)
INSERT INTO @Templates
VALUES ('MultiPaymentConfirmationCustomerFirstRowDetail', '', 1, '
<tr class="dataRow">
<td width="375">{CustomerNumber}/{CustomerBranch} - {CustomerName} </td>
<td>{InvoiceType} </td>
<td>{InvoiceNumber} </td>
<td align="right">{InvoiceDate:MM/dd/yyyy} </td>
<td align="right">{DueDate:MM/dd/yyyy} </td>
<td align="right" nowrap="">${PaymentAmount:f2} </td>
</tr>', getdate(), getdate(), 0)
INSERT INTO @Templates
VALUES ('MultiPaymentConfirmationCustomerNextRowsDetail', '', 1, '
<tr class="dataRow">
<td></td>
<td>{InvoiceType} </td>
<td>{InvoiceNumber} </td>
<td align="right">{InvoiceDate:MM/dd/yyyy} </td>
<td align="right">{DueDate:MM/dd/yyyy} </td>
<td align="right" nowrap="">${PaymentAmount:f2} </td>
</tr>', getdate(), getdate(), 0)
INSERT INTO @Templates
VALUES ('MultiPaymentConfirmationCustomerAltNextRowsDetail', '', 1, '
<tr class="altDataRow">
<td></td>
<td>{InvoiceType} </td>
<td>{InvoiceNumber} </td>
<td align="right">{InvoiceDate:MM/dd/yyyy} </td>
<td align="right">{DueDate:MM/dd/yyyy} </td>
<td align="right" nowrap="">${PaymentAmount:f2} </td>
</tr>', getdate(), getdate(), 0)
INSERT INTO @Templates
VALUES ('MultiPaymentConfirmationCustomerFooterAccount', '', 1, '<tr class="tableFoot">
<td>{BankName} - {AccountNumber} Total </td>
<td colspan="4">&nbsp; </td>
<td align="right" nowrap="">${AccountSum:f2} </td>
</tr>', getdate(), getdate(), 0)
INSERT INTO @Templates
VALUES ('MultiPaymentConfirmationCustomerFooterCustomer', '', 1, '<tr class="tableFoot">
<td>{CustomerNumber}/{CustomerBranch} - {CustomerName} Total </td>
<td colspan="4">&nbsp; </td>
<td align="right" nowrap="">${CustomerSum:f2} </td>
</tr>', getdate(), getdate(), 0)
INSERT INTO @Templates
VALUES ('MultiPaymentConfirmationCustomerFooterGrand', '', 1, '
<table cellpadding="3" cellspacing="0" border="0" width="700"><tr class="tableFoot">
<td width="375">Grand total being collected on {ScheduledDate:MM/dd/yyyy} </td>
<td align="right" nowrap="">${GrandSum:f2} </td>
</tr></table>', getdate(), getdate(), 0)
INSERT INTO @Templates
VALUES ('MultiPaymentConfirmationCustomerFooterEnd', '', 1, '</table>', getdate(), getdate(), 0)
INSERT INTO @Templates
VALUES ('OrderConfirmation', 'Ben E. Keith: {OrderStatus} for {CustomerNumber}-{CustomerName}; {InvoiceNumber}', 1, '{NotifHeader}<table style="width: 100%;">
   <tr>
       <td>Delivery Date: {ShipDate}</td>
       <td style="text-align:right;">{Count} Items / {PcsCount} Pieces</td>
   </tr>
   <tr>
       <td>Invoice Number: {InvoiceNumber}</td>
       <td style="text-align:right;">Invoice Total: ${Total}</td>
   </tr>
</table>
<table style="width: 100%;">
	<tr style="border-bottom:1px solid gray;">
		<th style="text-align:left;">Item # </th>
		<th style="text-align:left;">Confirmed Items </th>
		<th style="text-align:left;">Brand </th>
		<th style="text-align:left;">Ordered </th>
		<th style="text-align:left;">Confirmed </th>
		<th style="text-align:left;">Pack </th>
		<th style="text-align:left;">Size </th>
		<th style="text-align:left;">Price </th>
		<th style="text-align:left;">Status</th>
	</tr>
{OrderConfirmationItems}
</table>', getdate(), getdate(), 0)
INSERT INTO @Templates
VALUES ('OrderChangeDetail', '', 1, '<tr>
      <td style="text-align:left;color:maroon;">{ProductNumber} </td>
		<td style="text-align:left;font-size:small;color:maroon;">{ProductDescription} </td>
        <td style="text-align:left;font-size:small;color:maroon;">{Brand} </td>
		<td style="text-align:left;font-size:small;color:maroon;">{Quantity} </td>
		<td style="text-align:left;font-size:small;color:maroon;">{Sent} </td>
        <td style="text-align:left;font-size:small;color:maroon;">{Pack} </td>
        <td style="text-align:left;font-size:small;color:maroon;">{Size} </td>
		<td style="text-align:left;font-size:small;color:maroon;">{Price} </td>
		<td style="text-align:left;font-size:small;color:maroon;">{Status}</td>
</tr>', getdate(), getdate(), 0)
INSERT INTO @Templates
VALUES ('OrderConfirmationItems', '', 1, '{OrderConfirmationItemDetail}', getdate(), getdate(), 0)
INSERT INTO @Templates
VALUES ('OrderConfirmationItemDetail', '', 1, '<tr>
        <td style="text-align:left;font-size:small;">{ProductNumber} </td>
        <td style="text-align:left;font-size:small;">{ProductDescription} </td>
        <td style="text-align:left;font-size:small;">{Brand} </td>
        <td style="text-align:left;font-size:small;">{Quantity} </td>
        <td style="text-align:left;font-size:small;">{Sent} </td>
        <td style="text-align:left;font-size:small;">{Pack} </td>
        <td style="text-align:left;font-size:small;">{Size} </td>
        <td style="text-align:left;font-size:small;">{Price} </td>
        <td style="text-align:left;font-size:small;">{Status}</td>
    </tr>', getdate(), getdate(), 0)
INSERT INTO @Templates
VALUES ('OrderConfirmationItemsOOS', '', 1, '{OrderConfirmationItemOOSDetail}', getdate(), getdate(), 0)
INSERT INTO @Templates
VALUES ('OrderConfirmationItemOOSDetail', '', 1, '<tr>
      <td style="text-align:left;color:maroon;">{ProductNumber} </td>
		<td style="text-align:left;font-size:small;color:maroon;">{ProductDescription} </td>
        <td style="text-align:left;font-size:small;color:maroon;">{Brand} </td>
		<td style="text-align:left;font-size:small;color:maroon;">{Quantity} </td>
		<td style="text-align:left;font-size:small;color:maroon;">{Sent} </td>
        <td style="text-align:left;font-size:small;color:maroon;">{Pack} </td>
        <td style="text-align:left;font-size:small;color:maroon;">{Size} </td>
		<td style="text-align:left;font-size:small;color:maroon;">{Price} </td>
		<td style="text-align:left;font-size:small;color:maroon;">{Status}</td>
</tr>', getdate(), getdate(), 0)
INSERT INTO @Templates
VALUES ('OrderRejected', 'Ben E. Keith: Order Rejected for {CustomerNumber}-{CustomerName}', 1, '{NotifHeader}Order Rejected: {SpecialInstructions}
', getdate(), getdate(), 0)
INSERT INTO @Templates
VALUES ('GuestUserWelcome', 'Welcome to Entrée', 0, 'Thank you for your interest in the Entrée System, Powered by Ben E. Keith.

If you have comments or questions, or would like someone to contact you, please e-mail us at {contactEmail}', getdate(), getdate(), 0)
INSERT INTO @Templates
VALUES ('CreatedUserWelcome', 'Welcome to Entrée', 1, '<div>
<p>
Welcome to Entrée!
</p>

<p>
An account has been created for you. Please use the following link to create a password for your account.
</p>
<p>
<a href="{resetLink}" target="_blank">{resetLink}</a>
</p>
<p>
If clicking the link doesn''t seem to work, you can copy and paste the link into your browser''s address window, or retype it there.
</p>
</div>
', getdate(), getdate(), 0)
INSERT INTO @Templates
VALUES ('ResetPasswordRequest', 'Ben E. Keith Entrée Password Reset', 1, '<div>
	<p>
        We received a request to reset the password associated with this e-mail address. If you made this request, please follow the instructions below.
    </p>
	<p>
		Click the link below to reset your password:
	</p>
	<p>
		<a href="{resetLink}" target="_blank">Click Here To Change Password</a>
	</p>
  <p>
    If you did not request to have your password reset you can safely ignore this email.
  </p>
  <p>
    If clicking the link doesn''t seem to work, you can copy and paste the link into your browser''s address window, or retype it there.
  </p>
  <p>
	The link will expire in 3 days, so be sure to use it right away
  </p>
</div>
', getdate(), getdate(), 0)
INSERT INTO @Templates
VALUES ('ResetPassword', 'Your Ben E. Keith Entrée password has been changed', 0, 'Your Entrée password has changed

You recently changed your password for Ben E. Keith''s Entrée system. If you feel this was done in error please contact support. 

Temporary password: {password}
Url: {url}', getdate(), getdate(), 0)
INSERT INTO @Templates
VALUES ('ForwardUserMessage', '', 1, 
'<p style="text-align:center;">Forwarded by {UserEmail}</p><hr/>{ForwardBody}', getdate(), getdate(), 0)

MERGE INTO [Configuration].[MessageTemplates] A
USING @Templates B ON (A.[TemplateKey] = B.[TemplateKey])
WHEN MATCHED THEN
    UPDATE SET A.[Subject] = B.[Subject], A.[IsBodyHtml] = B.[IsBodyHtml], A.[Body] = B.[Body], A.[ModifiedUtc] = B.[ModifiedUtc], A.[Type] = B.[Type]
WHEN NOT MATCHED THEN
    INSERT ([TemplateKey],[Subject],[IsBodyHtml],[Body],[CreatedUtc],[ModifiedUtc],[Type]) 
	  VALUES(B.[TemplateKey],B.[Subject],B.[IsBodyHtml],B.[Body],B.[CreatedUtc],B.[ModifiedUtc],B.[Type]);