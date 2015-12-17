  INSERT INTO [BEK_Commerce_AppData].[Configuration].[MessageTemplates]
VALUES ('OrderConfirmation', 'Ben E. Keith: {OrderStatus} for {CustomerNumber}-{CustomerName}', 1, '<table style="width: 100%;">
<tr>
<td><h3>Thank you for your order.</h3></td>
<td style="text-align:right;"><h3>{CustomerName}</h3></td>
</tr>
<tr>
<td>Deliver Date: {ShipDate}</td>
<td style="text-align:right;">Customer # {CustomerNumber}</td>
</tr>
<tr>
<td>Items: {Count}</td>
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
</table>', '2015-11-06', '2015-11-06', 0)
INSERT INTO [BEK_Commerce_AppData].[Configuration].[MessageTemplates]
VALUES ('OrderConfirmationItemDetail', '', 1, '    <tr>
		<td style="text-align:left;">{ProductNumber} </td>
		<td style="text-align:left;">{ProductDescription} </td>
		<td style="text-align:left;">{Quantity} </td>
		<td style="text-align:left;">{Sent} </td>
		<td style="text-align:left;">{Price} </td>
		<td style="text-align:left;">{Status}</td>
	</tr>', '2015-11-06', '2015-11-06', 0)
INSERT INTO [BEK_Commerce_AppData].[Configuration].[MessageTemplates]
VALUES ('OrderConfirmationItemOOSDetail', '', 1, '    <tr>
		<td style="text-align:left;color:maroon;">{ProductNumber} </td>
		<td style="text-align:left;color:maroon;">{ProductDescription} </td>
		<td style="text-align:left;color:maroon;">{Quantity} </td>
		<td style="text-align:left;color:maroon;">{Sent} </td>
		<td style="text-align:left;color:maroon;">{Price} </td>
		<td style="text-align:left;color:maroon;">{Status}</td>
	</tr>', '2015-11-06', '2015-11-06', 0)
INSERT INTO [BEK_Commerce_AppData].[Configuration].[MessageTemplates]
VALUES ('OrderRejected', '', 1, '<h3 style="color:maroon;">{SpecialInstructions}</h3>', '2015-11-06', '2015-11-06', 0)
