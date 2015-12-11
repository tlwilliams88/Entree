using KeithLink.Common.Impl.Logging;
using KeithLink.Svc.FoundationSvc.Interface;

using CommerceServer.Core;
using CommerceServer.Core.Runtime.Orders;
using CommerceServer.Foundation;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.ServiceModel.Activation;
using System.Text;
using System.Web;
using System.Reflection;
using System.Xml;

namespace KeithLink.Svc.FoundationSvc
{
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class BEKFoundationService : OperationService, IBEKFoundationService
    {
        #region attributes
        private static string applicationNameForLogging = "Entree Foundation Service";
        #endregion

        #region methods
        public string SaveCartAsOrder(Guid userId, Guid cartId)
        {
            try
            {
                var context = Extensions.SiteHelper.GetOrderContext();

                var basket = context.GetBasket(userId, cartId);

                int startIndex = 1; // main frame needs these to start at 1
                foreach (LineItem lineItem in basket.OrderForms[0].LineItems)
                {
                    lineItem["LinePosition"] = startIndex;
                    startIndex++;
                }

                PipelineHelper pipeLineHelper = new PipelineHelper(Extensions.SiteHelper.GetSiteName());
                pipeLineHelper.RunPipeline(basket, true, false, "Checkout", string.Format("{0}\\pipelines\\checkout.pcf", HttpContext.Current.Server.MapPath(".")));

                basket.TrackingNumber = GetNextControlNumber();
                basket["OriginalOrderNumber"] = basket.TrackingNumber;
                var purchaseOrder = basket.SaveAsOrder();

                return purchaseOrder.TrackingNumber;
            }
            catch (Exception ex)
            {
                EventLogRepositoryImpl eventLog = new EventLogRepositoryImpl(applicationNameForLogging);
                eventLog.WriteErrorLog("Error in SaveCartAsOrder: ", ex);

                throw ex;
            }
        }

        public string SaveOrderAsChangeOrder(Guid userId, Guid cartId)
        {
            try
            {
                PurchaseOrder po = GetPurchaseOrder(userId, cartId);

                PipelineHelper pipeLineHelper = new PipelineHelper(Extensions.SiteHelper.GetSiteName());
                pipeLineHelper.RunPipeline(po, true, false, "Checkout", string.Format("{0}\\pipelines\\checkout.pcf", HttpContext.Current.Server.MapPath(".")));

                po.TrackingNumber = GetNextControlNumber();
                po.Status = "Submitted";
                po.Save();

                return po.TrackingNumber;
            }
            catch (Exception ex)
            {
                EventLogRepositoryImpl eventLog = new EventLogRepositoryImpl(applicationNameForLogging);
                eventLog.WriteErrorLog("Error in SaveOrderAsChangeOrder: ", ex);

                throw ex;
            }
        }

        public void CleanUpChangeOrder(Guid userId, Guid cartId)
        {
            try
            {
                PurchaseOrder po = GetPurchaseOrder(userId, cartId);

                List<LineItem> lineItemIndexesToRemove = new List<LineItem>();

                foreach (LineItem li in po.OrderForms[0].LineItems)
                {
                    if (li.Status == "deleted")
                    {
                        lineItemIndexesToRemove.Add(li);
                    }
                    li.Status = string.Empty;
                }

                foreach (LineItem item in lineItemIndexesToRemove)
                {
                    po.OrderForms[0].LineItems.Remove(item);
                }

                po.Save();
            }
            catch (Exception ex)
            {
                EventLogRepositoryImpl eventLog = new EventLogRepositoryImpl(applicationNameForLogging);
                eventLog.WriteErrorLog("Error in CleanUpChangeOrder: ", ex);

                throw ex;
            }
        }

        private static CommerceServer.Core.Runtime.Orders.PurchaseOrder GetPurchaseOrder(Guid userId, Guid cartId)
        {
            try
            {
                CommerceServer.Core.Runtime.Orders.OrderContext context = Extensions.SiteHelper.GetOrderContext();
                return context.GetPurchaseOrder(userId, cartId);
            }
            catch (Exception ex)
            {
                KeithLink.Common.Impl.Logging.EventLogRepositoryImpl eventLog =
                    new Common.Impl.Logging.EventLogRepositoryImpl(applicationNameForLogging);
                eventLog.WriteErrorLog("Error in GetPurchaseOrder: ", ex);

                throw ex;
            }
        }

        public string UpdatePurchaseOrder(Guid userId, Guid orderId, DateTime requestedShipDate, List<PurchaseOrderLineItemUpdate> lineItemUpdates)
        {
            try
            {
                CommerceServer.Core.Runtime.Orders.PurchaseOrder po = GetPurchaseOrder(userId, orderId);
                CommerceServer.Core.Runtime.Orders.LineItem[] lineItems = new CommerceServer.Core.Runtime.Orders.LineItem[po.OrderForms[0].LineItems.Count];
                po.OrderForms[0].LineItems.CopyTo(lineItems, 0);
                po["RequestedShipDate"] = requestedShipDate;
                int linePosition = 1; // main frame needs these to start at 1
                foreach (var li in lineItems)
                    if (linePosition < (int)li["LinePosition"])
                        linePosition = (int)li["LinePosition"];
                linePosition++;

                foreach (PurchaseOrderLineItemUpdate i in lineItemUpdates)
                {
                    CommerceServer.Core.Runtime.Orders.LineItem lineItem = lineItems.Where(x => x.ProductId == i.ItemNumber).FirstOrDefault();
                    // find existing item based on item number
                    if (i.Status == "changed" && lineItem != null)
                    {
                        lineItem.Quantity = i.Quantity;
                        lineItem["Each"] = i.Each;
                        lineItem["CatchWeight"] = i.CatchWeight;
                        lineItem.Status = "changed";
                    }
                    if (i.Status == "deleted" && lineItem != null)
                    {
                        lineItem.Status = "deleted";
                    }
                    if (i.Status == "added" && lineItem == null)
                    {
                        CommerceServer.Core.Runtime.Orders.LineItem li = new CommerceServer.Core.Runtime.Orders.LineItem() { ProductId = i.ItemNumber, Quantity = i.Quantity, Status = "added" };
                        li["CatchWeight"] = i.CatchWeight;
                        li["Each"] = i.Each;
                        li["Notes"] = string.Empty;
                        li["LinePosition"] = linePosition;
                        li.ProductCatalog = i.Catalog;
                        li.Status = "added";
                        linePosition++;
                        po.OrderForms[0].LineItems.Add(li);
                    }
                    if (i.Status == "added" && lineItem != null)
                    {
                        lineItem.Quantity = i.Quantity;
                        lineItem["Each"] = i.Each;
                        lineItem["CatchWeight"] = i.CatchWeight;
                    }

                }

                if (po.Status.StartsWith("confirmed", StringComparison.InvariantCultureIgnoreCase))
                {
                    po.Status = "Confirmed with un-submitted changes";
                }

                PipelineHelper pipeLineHelper = new PipelineHelper(Extensions.SiteHelper.GetSiteName());
                pipeLineHelper.RunPipeline(po, true, false, "Checkout", string.Format("{0}\\pipelines\\checkout.pcf", HttpContext.Current.Server.MapPath(".")));

                po.Save();
                return po.TrackingNumber;
            }
            catch (Exception ex)
            {
                KeithLink.Common.Impl.Logging.EventLogRepositoryImpl eventLog =
                    new Common.Impl.Logging.EventLogRepositoryImpl(applicationNameForLogging);
                eventLog.WriteErrorLog("Error in UpdatePurchaseOrder: ", ex);

                throw ex;
            }
        }

        public string CancelPurchaseOrder(Guid userId, Guid orderId)
        {
            try
            {
                CommerceServer.Core.Runtime.Orders.PurchaseOrder po = GetPurchaseOrder(userId, orderId);
                po.Status = "Cancelled";
                po.TrackingNumber = GetNextControlNumber();
                po.Save();
                return po.TrackingNumber;
            }
            catch (Exception ex)
            {
                KeithLink.Common.Impl.Logging.EventLogRepositoryImpl eventLog =
                    new Common.Impl.Logging.EventLogRepositoryImpl(applicationNameForLogging);
                eventLog.WriteErrorLog("Error in CancelPurchaseOrder: ", ex);

                throw ex;
            }
        }

        private static string GetNextControlNumber()
        {
            try
            {
                string controlNumber = string.Empty;
                // get tracking number from DB
                using (SqlConnection conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["AppDataConnection"].ConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("Orders.usp_GetNextControlNumber", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        SqlParameter parm = new SqlParameter();
                        parm.Direction = ParameterDirection.ReturnValue;
                        cmd.Parameters.Add(parm);
                        conn.Open();
                        cmd.ExecuteNonQuery();
                        controlNumber = (int.Parse(cmd.Parameters[0].Value.ToString())).ToString("0000000.##"); // format to main frame of 7 digits
                    }
                }
                return controlNumber;
            }
            catch (Exception ex)
            {
                KeithLink.Common.Impl.Logging.EventLogRepositoryImpl eventLog =
                    new Common.Impl.Logging.EventLogRepositoryImpl(applicationNameForLogging);
                eventLog.WriteErrorLog("Error in GetNextControlNumber: ", ex);

                throw ex;
            }
        }

        public System.Xml.XmlElement GetUnconfirmatedOrders()
        {
            var manager = Extensions.SiteHelper.GetOrderManageContext().PurchaseOrderManager;
            System.Data.DataSet searchableProperties = manager.GetSearchableProperties(CultureInfo.CurrentUICulture.ToString());
            SearchClauseFactory searchClauseFactory = manager.GetSearchClauseFactory(searchableProperties, "PurchaseOrder");
            SearchClause clause = searchClauseFactory.CreateClause(ExplicitComparisonOperator.Equal, "Status", "Submitted");
            DataSet results = manager.SearchPurchaseOrders(clause, new SearchOptions() { NumberOfRecordsToReturn = 100, PropertiesToReturn = "OrderGroupId,LastModified,SoldToId" });

            int c = results.Tables.Count;

            // Get the value of the OrderGroupId property of each
            // purchase order.
            List<Guid> poIds = new List<Guid>();
            foreach (DataRow row in results.Tables[0].Rows)
            {
                poIds.Add(new Guid(row["OrderGroupId"].ToString()));
            }
            // Get the XML representation of the purchase orders.
            return manager.GetPurchaseOrdersAsXml(poIds.ToArray());
        }

        public string CheckForLostOrders(out string sBody)
        {
            StringBuilder sbMsgSubject = new StringBuilder();
            StringBuilder sbMsgBody = new StringBuilder();

            CheckForLostOrdersByStatus(sbMsgSubject, sbMsgBody, "Pending");
            CheckForLostOrdersByStatus(sbMsgSubject, sbMsgBody, "Submitted");

            sBody = sbMsgBody.ToString();
            return sbMsgSubject.ToString();
        }

        private void CheckForLostOrdersByStatus(StringBuilder sbMsgSubject, StringBuilder sbMsgBody, string qStatus)
        {
            List<System.Xml.XmlElement> Pos;
            GetPurchaseOrdersByStatus(qStatus, out Pos);
            StringBuilder sbAppendSubject;
            StringBuilder sbAppendBody;
            BuildAlertStringsForLostPurchaseOrders(out sbAppendSubject, out sbAppendBody, Pos, qStatus);
            if (sbAppendSubject.Length > 0)
            {
                sbMsgSubject.Append(sbAppendSubject.ToString());
            }
            if (sbAppendBody.Length > 0)
            {
                sbMsgBody.Append(sbAppendBody.ToString());
            }
        }

        private void GetPurchaseOrdersByStatus(string queryStatus, out List<System.Xml.XmlElement> Pos)
        {
            var manager = Extensions.SiteHelper.GetOrderManageContext().PurchaseOrderManager;
            System.Data.DataSet searchableProperties = manager.GetSearchableProperties(CultureInfo.CurrentUICulture.ToString());
            // set what to search
            SearchClauseFactory searchClauseFactory = manager.GetSearchClauseFactory(searchableProperties, "PurchaseOrder");
            // set what field/value to search for
            SearchClause clause = searchClauseFactory.CreateClause(ExplicitComparisonOperator.Equal, "Status", queryStatus);
            // set what fields to return
            DataSet results = manager.SearchPurchaseOrders(clause, new SearchOptions() { NumberOfRecordsToReturn = 100, PropertiesToReturn = "OrderGroupId,LastModified,SoldToId" });

            int c = results.Tables.Count;

            Pos = new List<System.Xml.XmlElement>();
            List<Guid> poIds = new List<Guid>();
            foreach (DataRow row in results.Tables[0].Rows)
            {
                poIds.Add(new Guid(row["OrderGroupId"].ToString()));
            }
            // Get the XML representation of the purchase orders.
            if (poIds.Count > 0)
            {
                foreach (var poid in poIds)
                {
                    List<Guid> single = new List<Guid>();
                    single.Add(poid);
                    var xml = manager.GetPurchaseOrdersAsXml(single.ToArray());
                    Pos.Add(xml);
                }
            }
        }

        private void BuildAlertStringsForLostPurchaseOrders(out StringBuilder sbSubject, out StringBuilder sbBody, List<System.Xml.XmlElement> Pos, string qStatus)
        {
            sbSubject = new StringBuilder();
            sbBody = new StringBuilder();
            if ((Pos != null) && (Pos.Count > 0))
            {
                foreach (var xml in Pos)
                {
                    XmlDocument doc = new XmlDocument();
                    doc.LoadXml(xml.InnerXml);
                    //var nRoot = doc.GetElementsByTagName("PurchaseOrder")[0];
                    DateTime lastModified;
                    DateTime.TryParse(doc.DocumentElement.Attributes.GetNamedItem("LastModified").Value, out lastModified);
                    // only if they've been created more than 10 minutes ago in the query status
                    if (lastModified < DateTime.Now.AddMinutes(-10))
                    {
                        sbSubject.Clear();
                        sbSubject.Append("PO in a " + qStatus + " status for more than 10 minutes.");
                        sbBody.Append("* PO");
                        sbBody.Append(" for ");
                        XmlNodeList childs = doc.DocumentElement.GetElementsByTagName("WeaklyTypedProperties");
                        XmlDocument elem = new XmlDocument();
                        elem.LoadXml("<Info>" + childs[childs.Count - 1].InnerXml + "</Info>");
                        {
                            foreach (XmlElement child in elem.DocumentElement.GetElementsByTagName("WeaklyTypedProperty"))
                            {
                                if (child.Attributes["Name"].Value.Equals("customerid", StringComparison.CurrentCultureIgnoreCase))
                                {
                                    sbBody.Append(child.Attributes["Value"].Value);
                                }
                            }
                        }
                        //sbBody.Append((doc.DocumentElement.GetElementsByTagName("WeaklyTypedProperties")[0]).g);
                        sbBody.Append("-");
                        {
                            foreach (XmlElement child in elem.DocumentElement.GetElementsByTagName("WeaklyTypedProperty"))
                            {
                                if (child.Attributes["Name"].Value.Equals("branchid", StringComparison.CurrentCultureIgnoreCase))
                                {
                                    sbBody.Append(child.Attributes["Value"].Value.ToUpper());
                                }
                            }
                        }
                        sbBody.Append(" with cart ");
                        {
                            sbBody.Append(doc.DocumentElement.Attributes["Name"].Value);
                        }
                        sbBody.Append(" and tracking ");
                        {
                            sbBody.Append(doc.DocumentElement.Attributes["TrackingNumber"].Value);
                        }
                        sbBody.Append(" last modified " + lastModified.ToShortDateString());
                        sbBody.Append(" at " + lastModified.ToShortTimeString());
                        sbBody.Append(" in status " + qStatus);
                        sbBody.Append(".\n");
                    }
                }
            }
        }

        #endregion
    }
}
// An example of each purchase order xml
//<PurchaseOrder BillingCurrency="" Status="Submitted" SoldToId="39f1ac2d-494a-4a65-bf83-68c020e3f815" TaxTotal="0.0000" 
//    LastModified="2015-11-17T13:01:04.887-06:00" Created="2015-11-17T13:01:04.887-06:00" StatusCode="PurchaseOrder" SoldToName="" 
//    TrackingNumber="0001083" SubTotal="130.8700" IsDirty="false" LineItemCount="4" OrderGroupId="520a5db1-7d8d-4f66-b906-d08b26a7f16a" 
//    HandlingTotal="0.0000" BasketId="655811bf-b90d-4804-894b-5aebf6425cc1" ShippingTotal="0.0000" ModifiedBy="" Total="130.8700" 
//    Name="sfdf_726971_NewCart0">
//    <OrderForms>
//        <OrderForm Status="InProcess" TaxTotal="0.0000" LastModified="2015-11-17T13:01:04.82-06:00" Created="2015-11-17T13:00:50.793-06:00" 
//            PromoUserIdentity="" SubTotal="130.8700" OrderGroupId="520a5db1-7d8d-4f66-b906-d08b26a7f16a" HandlingTotal="0.0000" 
//            ShippingTotal="0.0000" ModifiedBy="" BillingAddressId="" Total="130.8700" OrderFormId="30cdac83-bc8d-4fcc-8e2f-61bb7b33fad4" 
//            Name="Default">
//            <PromoCodeRecords />
//            <Shipments />
//            <PromoCodes />
//            <Payments />
//            <LineItems>
//                <LineItem ShippingMethodName="" ProductId="630420" PlacedPrice="50.2000" Description="" InventoryCondition="InStock" 
//                    OrderFormId="30cdac83-bc8d-4fcc-8e2f-61bb7b33fad4" ShippingAddressId="" AllowBackordersAndPreorders="true" 
//                    LastModified="2015-11-17T13:01:04.82-06:00" OrderGroupId="520a5db1-7d8d-4f66-b906-d08b26a7f16a" ListPrice="50.2000" 
//                    Quantity="1.0000" DisplayName="Drink Mix Fruit Punch" ModifiedBy="" ProductCatalog="fdf" 
//                    ShippingMethodId="00000000-0000-0000-0000-000000000000" Created="2015-11-17T13:00:50.847-06:00" 
//                    OrderLevelDiscountAmount="0.0000" PreorderQuantity="0.0000" ExtendedPrice="0.0000" Status="" 
//                    BackorderQuantity="0.0000" LineItemDiscountAmount="0.0000" InStockQuantity="0.0000" 
//                    LineItemId="72c4ea80-dd99-482a-970a-f1079520379e">
//                    <ItemLevelDiscountsApplied />
//                    <OrderLevelDiscountsApplied />
//                    <WeaklyTypedProperties>
//                        <WeaklyTypedProperty Name="CatchWeight" Value="false" Type="Boolean" />
//                        <WeaklyTypedProperty Name="Notes" Value="test note" Type="String" />
//                        <WeaklyTypedProperty Name="Each" Value="false" Type="Boolean" />
//                        <WeaklyTypedProperty Name="IsCombinedProperty" Value="false" Type="Boolean" />
//                        <WeaklyTypedProperty Name="ParLevel" Value="0" Type="Decimal" />
//                        <WeaklyTypedProperty Name="LinePosition" Value="1" Type="Int32" />
//                        <WeaklyTypedProperty Name="Label" Value="cat 1" Type="String" />
//                    </WeaklyTypedProperties>
//                </LineItem>
//                <LineItem ShippingMethodName="" ProductId="102968" PlacedPrice="19.8500" Description="" InventoryCondition="InStock" 
//                    OrderFormId="30cdac83-bc8d-4fcc-8e2f-61bb7b33fad4" ShippingAddressId="" AllowBackordersAndPreorders="true" 
//                    LastModified="2015-11-17T13:01:04.82-06:00" OrderGroupId="520a5db1-7d8d-4f66-b906-d08b26a7f16a" ListPrice="19.8500" 
//                    Quantity="1.0000" DisplayName="Juice Cranberry Cocktail 10%" ModifiedBy="" ProductCatalog="fdf" 
//                    ShippingMethodId="00000000-0000-0000-0000-000000000000" Created="2015-11-17T13:00:50.86-06:00" 
//                    OrderLevelDiscountAmount="0.0000" PreorderQuantity="0.0000" ExtendedPrice="0.0000" Status="" 
//                    BackorderQuantity="0.0000" LineItemDiscountAmount="0.0000" InStockQuantity="0.0000" 
//                    LineItemId="744a1c39-bf4b-4a6d-befa-14f1246b1dcc">
//                    <ItemLevelDiscountsApplied />
//                    <OrderLevelDiscountsApplied />
//                    <WeaklyTypedProperties>
//                        <WeaklyTypedProperty Name="CatchWeight" Value="false" Type="Boolean" />
//                        <WeaklyTypedProperty Name="IsCombinedProperty" Value="false" Type="Boolean" />
//                        <WeaklyTypedProperty Name="Each" Value="false" Type="Boolean" />
//                        <WeaklyTypedProperty Name="ParLevel" Value="0" Type="Decimal" />
//                        <WeaklyTypedProperty Name="LinePosition" Value="2" Type="Int32" />
//                        <WeaklyTypedProperty Name="Label" Value="cat 1" Type="String" />
//                    </WeaklyTypedProperties>
//                </LineItem>
//                <LineItem ShippingMethodName="" ProductId="630613" PlacedPrice="53.1500" Description="" InventoryCondition="InStock" 
//                    OrderFormId="30cdac83-bc8d-4fcc-8e2f-61bb7b33fad4" ShippingAddressId="" 
//                    AllowBackordersAndPreorders="true" LastModified="2015-11-17T13:01:04.82-06:00" 
//                    OrderGroupId="520a5db1-7d8d-4f66-b906-d08b26a7f16a" ListPrice="53.1500" Quantity="1.0000" 
//                    DisplayName="Gatorade Fierce Grape" ModifiedBy="" ProductCatalog="fdf" 
//                    ShippingMethodId="00000000-0000-0000-0000-000000000000" Created="2015-11-17T13:00:50.863-06:00" 
//                    OrderLevelDiscountAmount="0.0000" PreorderQuantity="0.0000" ExtendedPrice="0.0000" Status="" 
//                    BackorderQuantity="0.0000" LineItemDiscountAmount="0.0000" InStockQuantity="0.0000" 
//                    LineItemId="0d2ccad1-ef11-400f-b699-a89f204b4ac5"><ItemLevelDiscountsApplied />
//                    <OrderLevelDiscountsApplied />
//                    <WeaklyTypedProperties>
//                        <WeaklyTypedProperty Name="CatchWeight" Value="false" Type="Boolean" />
//                        <WeaklyTypedProperty Name="IsCombinedProperty" Value="false" Type="Boolean" />
//                        <WeaklyTypedProperty Name="Each" Value="false" Type="Boolean" />
//                        <WeaklyTypedProperty Name="ParLevel" Value="0" Type="Decimal" />
//                        <WeaklyTypedProperty Name="LinePosition" Value="3" Type="Int32" />
//                        <WeaklyTypedProperty Name="Label" Value="cat 1" Type="String" />
//                    </WeaklyTypedProperties>
//                </LineItem>
//                <LineItem ShippingMethodName="" ProductId="098073" PlacedPrice="7.6700" Description="" InventoryCondition="InStock" 
//                    OrderFormId="30cdac83-bc8d-4fcc-8e2f-61bb7b33fad4" ShippingAddressId="" AllowBackordersAndPreorders="true" 
//                    LastModified="2015-11-17T13:01:04.82-06:00" OrderGroupId="520a5db1-7d8d-4f66-b906-d08b26a7f16a" ListPrice="7.6700" 
//                    Quantity="1.0000" DisplayName="Carrot Match Stick" ModifiedBy="" ProductCatalog="fdf" 
//                    ShippingMethodId="00000000-0000-0000-0000-000000000000" Created="2015-11-17T13:00:50.863-06:00" 
//                    OrderLevelDiscountAmount="0.0000" PreorderQuantity="0.0000" ExtendedPrice="0.0000" Status="" 
//                    BackorderQuantity="0.0000" LineItemDiscountAmount="0.0000" InStockQuantity="0.0000" 
//                    LineItemId="f12f38bb-1249-4c01-9062-c691b7cb4f34">
//                    <ItemLevelDiscountsApplied />
//                    <OrderLevelDiscountsApplied />
//                    <WeaklyTypedProperties>
//                        <WeaklyTypedProperty Name="CatchWeight" Value="false" Type="Boolean" />
//                        <WeaklyTypedProperty Name="IsCombinedProperty" Value="false" Type="Boolean" />
//                        <WeaklyTypedProperty Name="Each" Value="true" Type="Boolean" />
//                        <WeaklyTypedProperty Name="ParLevel" Value="0" Type="Decimal" />
//                        <WeaklyTypedProperty Name="LinePosition" Value="4" Type="Int32" />
//                        <WeaklyTypedProperty Name="Label" Value="cat 2" Type="String" />
//                    </WeaklyTypedProperties>
//                </LineItem>
//            </LineItems>
//            <WeaklyTypedProperties>
//                <WeaklyTypedProperty Name="BranchId" Value="fdf" Type="String" />
//                <WeaklyTypedProperty Name="Shared" Value="true" Type="Boolean" />
//                <WeaklyTypedProperty Name="ListType" Value="3" Type="Int32" />
//                <WeaklyTypedProperty Name="BasketLevelDiscountsTotal" Value="0" Type="Decimal" />
//                <WeaklyTypedProperty Name="DiscountsTotal" Value="0" Type="Decimal" />
//                <WeaklyTypedProperty Name="CustomerId" Value="726971" Type="String" />
//                <WeaklyTypedProperty Name="ShippingDiscountsTotal" Value="0" Type="Decimal" />
//                <WeaklyTypedProperty Name="TempSubTotal" Value="130.87" Type="Decimal" />
//                <WeaklyTypedProperty Name="RequestedShipDate" Value="2015-11-18T00:00:00-06:00" Type="DateTime" />
//                <WeaklyTypedProperty Name="LineItemDiscountsTotal" Value="0" Type="Decimal" />
//                <WeaklyTypedProperty Name="DisplayName" Value="New Cart 0" Type="String" />
//                <WeaklyTypedProperty Name="BasketType" Value="0" Type="Int32" />
//            </WeaklyTypedProperties>
//        </OrderForm>
//    </OrderForms>
//    <Addresses />
//    <WeaklyTypedProperties>
//        <WeaklyTypedProperty Name="OriginalOrderNumber" Value="0001083" Type="String" />
//        <WeaklyTypedProperty Name="DiscountsTotal" Value="0" Type="Decimal" />
//        <WeaklyTypedProperty Name="ListType" Value="3" Type="Int32" />
//        <WeaklyTypedProperty Name="BasketLevelDiscountsTotal" Value="0" Type="Decimal" />
//        <WeaklyTypedProperty Name="BranchId" Value="fdf" Type="String" />
//        <WeaklyTypedProperty Name="RequestedShipDate" Value="2015-11-18T00:00:00-06:00" Type="DateTime" />
//        <WeaklyTypedProperty Name="ShippingDiscountsTotal" Value="0" Type="Decimal" />
//        <WeaklyTypedProperty Name="LineItemDiscountsTotal" Value="0" Type="Decimal" />
//        <WeaklyTypedProperty Name="TempSubTotal" Value="130.87" Type="Decimal" />
//        <WeaklyTypedProperty Name="DisplayName" Value="New Cart 0" Type="String" />
//        <WeaklyTypedProperty Name="Shared" Value="true" Type="Boolean" />
//        <WeaklyTypedProperty Name="CustomerId" Value="726971" Type="String" />
//        <WeaklyTypedProperty Name="BasketType" Value="0" Type="Int32" />
//    </WeaklyTypedProperties>
//</PurchaseOrder>
