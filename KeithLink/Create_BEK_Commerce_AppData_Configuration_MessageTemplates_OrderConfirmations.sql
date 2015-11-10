INSERT INTO [BEK_Commerce_AppData].[Configuration].[MessageTemplates]
VALUES ('OrderConfirmation', 'Order Confirmation for {CustomerNumber}-{CustomerName}', 1, '<table style="width: 100%;">
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
		<th>Item # </th>
		<th>Confirmed Items </th>
		<th>Ordered </th>
		<th>Price </th>
		<th>Status</th>
	</tr>
{OrderConfirmationItemDetail}
</table>', '2015-11-06', '2015-11-06', 0)
INSERT INTO [BEK_Commerce_AppData].[Configuration].[MessageTemplates]
VALUES ('OrderConfirmationItemDetail', '', 1, '    <tr>
		<td>{ProductNumber} </td>
		<td>{ProductDescription} </td>
		<td>{Quantity} </td>
		<td>{Price} </td>
		<td>{Status}</td>
	</tr>', '2015-11-06', '2015-11-06', 0)
INSERT INTO [BEK_Commerce_AppData].[Configuration].[MessageTemplates]
VALUES ('OrderConfirmationItemsOOS', '', 1, '<table style="width: 100%;">
	<tr style="border-bottom:1px solid maroon;">
		<th style="color:maroon;">Item # </th>
		<th style="color:maroon;">Exception Items </th>
		<th style="color:maroon;">Ordered </th>
		<th style="color:maroon;">Confirmed </th>
		<th style="color:maroon;">Price/Extended </th>
		<th style="color:maroon;">Status</th>
	</tr>
{OrderConfirmationItemOOSDetail}
</table><p></p>', '2015-11-06', '2015-11-06', 0)
INSERT INTO [BEK_Commerce_AppData].[Configuration].[MessageTemplates]
VALUES ('OrderConfirmationItemOOSDetail', '', 1, '    <tr>
		<td style="color:maroon;">{ProductNumber} </td>
		<td style="color:maroon;">{ProductDescription} </td>
		<td style="color:maroon;">{Quantity} </td>
		<td style="color:maroon;">{Sent} </td>
		<td style="color:maroon;">{Price} </td>
		<td style="color:maroon;">{Status}</td>
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
