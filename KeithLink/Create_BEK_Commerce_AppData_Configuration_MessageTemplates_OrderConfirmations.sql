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
{OrderConfirmationItems}', '2015-11-06', '2015-11-06', 0)
INSERT INTO [BEK_Commerce_AppData].[Configuration].[MessageTemplates]
VALUES ('OrderConfirmationItems', '', 1, '<table style="width: 100%;">
	<tr style="border-bottom:1px solid gray;">
		<th style="text-align:right;">Item # </th>
		<th style="text-align:right;">Confirmed Items </th>
		<th style="text-align:right;">Ordered </th>
		<th style="text-align:right;">Price </th>
		<th style="text-align:right;">Status</th>
	</tr>
{OrderConfirmationItemDetail}
</table>', '2015-11-06', '2015-11-06', 0)
INSERT INTO [BEK_Commerce_AppData].[Configuration].[MessageTemplates]
VALUES ('OrderConfirmationItemDetail', '', 1, '    <tr>
		<td style="text-align:right;">{ProductNumber} </td>
		<td style="text-align:right;">{ProductDescription} </td>
		<td style="text-align:right;">{Quantity} </td>
		<td style="text-align:right;">{Price} </td>
		<td style="text-align:right;">{Status}</td>
	</tr>', '2015-11-06', '2015-11-06', 0)
INSERT INTO [BEK_Commerce_AppData].[Configuration].[MessageTemplates]
VALUES ('OrderConfirmationItemsOOS', '', 1, '<table style="width: 100%;">
	<tr style="border-bottom:1px solid maroon;">
		<th style="text-align:right;color:maroon;">Item # </th>
		<th style="text-align:right;color:maroon;">Exception Items </th>
		<th style="text-align:right;color:maroon;">Ordered </th>
		<th style="text-align:right;color:maroon;">Confirmed </th>
		<th style="text-align:right;color:maroon;">Price </th>
		<th style="color:maroon;">Status</th>
	</tr>
{OrderConfirmationItemOOSDetail}
</table><p></p>', '2015-11-06', '2015-11-06', 0)
INSERT INTO [BEK_Commerce_AppData].[Configuration].[MessageTemplates]
VALUES ('OrderConfirmationItemOOSDetail', '', 1, '    <tr>
		<td style="text-align:right;color:maroon;">{ProductNumber} </td>
		<td style="text-align:right;color:maroon;">{ProductDescription} </td>
		<td style="text-align:right;color:maroon;">{Quantity} </td>
		<td style="text-align:right;color:maroon;">{Sent} </td>
		<td style="text-align:right;color:maroon;">{Price} </td>
		<td style="text-align:right;color:maroon;">{Status}</td>
	</tr>', '2015-11-06', '2015-11-06', 0)
INSERT INTO [BEK_Commerce_AppData].[Configuration].[MessageTemplates]
VALUES ('OrderSuccessful', '', 1, 'Order updated from status: {OriginalStatus} to {CurrentStatus}', '2015-11-06', '2015-11-06', 0)
INSERT INTO [BEK_Commerce_AppData].[Configuration].[MessageTemplates]
VALUES ('OrderRejected', '', 1, '<h3 style="color:maroon;">{SpecialInstructions}</h3>', '2015-11-06', '2015-11-06', 0)
INSERT INTO [BEK_Commerce_AppData].[Configuration].[MessageTemplates]
VALUES ('OrderChange', '', 1, 'Order Changes
<table style="width: 100%">
	<tr>
		<th>Number </th>
		<th>Status</th>
	</tr>
{OrderChangeLines}
</table><p></p>', '2015-11-06', '2015-11-06', 0)
INSERT INTO [BEK_Commerce_AppData].[Configuration].[MessageTemplates]
VALUES ('OrderChangeDetail', '', 1, '    <tr>
		<td>{Number} </td>
		<td>{Status}</td>
	</tr>', '2015-11-06', '2015-11-06', 0)
