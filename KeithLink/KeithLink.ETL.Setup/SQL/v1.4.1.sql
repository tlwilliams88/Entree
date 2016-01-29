--Modify Order Confirmation Templates to add invoice number, BAKillins, 1/12/2016
update [BEK_Commerce_AppData].[Configuration].[MessageTemplates]
  set Subject = 'Ben E. Keith: {OrderStatus} for {CustomerNumber}-{CustomerName};{InvoiceNumber}'
    , Body = '<table style="width: 100%;">
   <tr>
       <td><h3>Thank you for your order.</h3></td>
       <td style="text-align:right;"><h3>{CustomerName}</h3></td>
   </tr>
   <tr>
       <td>Delivery Date: {ShipDate}</td>
       <td style="text-align:right;">Customer # {CustomerNumber}</td>
   </tr>
   <tr>
       <td>Items: {Count}</td>
       <td style="text-align:right;">Invoice Number: {InvoiceNumber}</td>
   </tr>
   <tr>
       <td></td>
       <td style="text-align:right;">Invoice Total: ${Total}</td>
   </tr>
</table>
<hr/>
<table style="width: 100%;">
	<tr style="border-bottom:1px solid gray;">
		<th style="text-align:left;">Item # </th>
		<th style="text-align:left;">Confirmed Items </th>
		<th style="text-align:left;">Ordered </th>
		<th style="text-align:left;">Confirmed </th>
		<th style="text-align:left;">Price </th>
		<th style="text-align:left;">Status</th>
	</tr>
{OrderConfirmationItems}
</table>
'
   , ModifiedUtc = '2016-01-12'
  where TemplateKey = 'OrderConfirmation'

update [BEK_Commerce_AppData].[Configuration].[MessageTemplates]
  set Body = '    <tr>
        <td style="text-align:left;">{ProductNumber} </td>
        <td style="text-align:left;">{ProductDescription} </td>
        <td style="text-align:left;">{Quantity} </td>
        <td style="text-align:left;">{Sent} </td>
        <td style="text-align:left;">{Price} </td>
        <td style="text-align:left;">{Status}</td>
    </tr>'
   , ModifiedUtc = '2016-01-12'
  where TemplateKey = 'OrderConfirmationItemDetail'

update [BEK_Commerce_AppData].[Configuration].[MessageTemplates]
  set Body = '<tr>
      <td style="text-align:left;color:maroon;">{ProductNumber} </td>
		<td style="text-align:left;color:maroon;">{ProductDescription} </td>
		<td style="text-align:left;color:maroon;">{Quantity} </td>
		<td style="text-align:left;color:maroon;">{Sent} </td>
		<td style="text-align:left;color:maroon;">{Price} </td>
		<td style="text-align:left;color:maroon;">{Status}</td>
</tr>'
   , ModifiedUtc = '2016-01-12'
  where TemplateKey = 'OrderConfirmationItemOOSDetail'

INSERT INTO [BEK_Commerce_AppData].[Configuration].[MessageTemplates]
VALUES ('OrderRejected', '', 1, '<h3 style="color:maroon;">{SpecialInstructions}</h3>', '2015-11-06', '2015-11-06', 0)
update [BEK_Commerce_AppData].[Configuration].[MessageTemplates]
  set  Subject = 'Ben E. Keith: Order Rejected for {CustomerNumber}-{CustomerName}'
   , Body = '<h3>{CustomerName}</h3><h3>Customer # {CustomerNumber}</h3><h3 style="color:maroon;">{SpecialInstructions}</h3>'
   , ModifiedUtc = '2016-01-12'
  where TemplateKey = 'OrderRejected'

