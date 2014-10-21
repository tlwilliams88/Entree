using KeithLink.Svc.InternalSvc.Interfaces;
using KeithLink.Svc.Core.Models.Confirmations;
using KeithLink.Common.Core.Extensions;
using CommerceServer.Core.Runtime.Orders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;


namespace KeithLink.Svc.InternalSvc
{
	// NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "PipelineService" in code, svc and config file together.
	// NOTE: In order to launch WCF Test Client for testing this service, please select PipelineService.svc or PipelineService.svc.cs at the Solution Explorer and start debugging.
	public class OrderService : IOrderService
	{
		public OrderService()
		{
			
		}

		public bool OrderConfirmation(ConfirmationFile confirmation)
		{
            return true;

            var siteName = System.Configuration.ConfigurationManager.AppSettings["CS_Sitename"].ToString();
            var context = CommerceServer.Core.Runtime.Orders.OrderContext.Create(siteName);

            var poNum = confirmation.Header.ConfirmationNumber;
            // get the guids for the customers associated users and loop if necessary
            PurchaseOrder po = context.GetPurchaseOrder(Guid.Parse("4065067C-BAE0-41CD-A2F9-E89F377D4386"), poNum);
            po["MasterNumber"] = "9865g";
            if (po == null)
            {
                // if no PO, silently ignore?  could be the case if multiple control numbers out at once...
            }
            else
            {
                // get header status into CS
                // values are " ", "P", "I", "D" = " " open, "P" Processing, "I" Invoicing, "D" Delete
                string trimmedConfirmationStatus = confirmation.Header.ConfirmationStatus.Trim().ToUpper();
                if (String.IsNullOrEmpty(trimmedConfirmationStatus))
                {
                    po.Status = "NewOrder";
                }
                else if (trimmedConfirmationStatus.Equals("P"))
                {
                    po.Status = "Submitted";
                }
                else if (trimmedConfirmationStatus.Equals("I"))
                {
                    po.Status = "InProcess";
                }
                else if (trimmedConfirmationStatus.Equals("D"))
                {
                    po.Status = "Cancelled";
                }

                LineItem[] lineItems = new LineItem[po.OrderForms.Count];
                po.OrderForms[0].LineItems.CopyTo(lineItems, 0);

                if (trimmedConfirmationStatus == "P" || trimmedConfirmationStatus == "I")
                {
                    foreach (var confirmationDetail in confirmation.Detail)
                    {
                        // match up to incoming line items to CS line items
                        string recNum = confirmationDetail.RecordNumber.Trim();
                        LineItem orderFormLineItem = lineItems[0];
                        string confirmationStatus = confirmationDetail.ConfirmationMessage.Trim().ToUpper();
                        if (String.IsNullOrEmpty(confirmationStatus))
                        {
                            orderFormLineItem.Status = "Shipped";
                        }
                        if (confirmationStatus == "P") // partial ship
                        {
                            orderFormLineItem.BackorderQuantity = confirmationDetail.QuantityOrdered - confirmationDetail.QuantityShipped;
                            orderFormLineItem.Status = "Partially Shipped";
                        }
                        else if (confirmationStatus == "O") // out of stock
                        {
                            orderFormLineItem.BackorderQuantity = confirmationDetail.QuantityOrdered - confirmationDetail.QuantityShipped;
                            orderFormLineItem.Status = "Out of Stock";
                        }
                        else if (confirmationStatus == "R") // item replaced
                        {
                            orderFormLineItem.Status = "Item Replaced";
                        }
                        else if (confirmationStatus == "Z") // item replaced, but replacement currently out of stock
                        {
                            orderFormLineItem.Status = "Item Replaced, Out of Stock";
                        }
                        else if (confirmationStatus == "T") // Item replaced, partial fill
                        {
                            orderFormLineItem.Status = "Partially Shipped, Item Replaced";
                        }
                        else if (confirmationStatus == "S") // item subbed
                        {
                            orderFormLineItem.Status = "Item Subbed";
                        }
                    }
                }
                po.Save();
            }
			return true;
		}
	}
}
