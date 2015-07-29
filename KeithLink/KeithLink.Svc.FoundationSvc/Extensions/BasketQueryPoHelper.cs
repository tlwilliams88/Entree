using KeithLink.Common.Core.Extensions;

using CommerceServer.Core.Runtime.Orders;
using CommerceServer.Foundation;
using CommerceServer.Foundation.SequenceComponents;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KeithLink.Svc.FoundationSvc.Extensions
{
    public class BasketQueryPoHelper : OperationSequenceComponent {
        #region attributes
        #endregion

        #region ctor
        public BasketQueryPoHelper() {
        }
        #endregion

        #region methods
        public override void ExecuteQuery(CommerceQueryOperation queryOperation, OperationCacheDictionary operationCache, CommerceQueryOperationResponse response) {
            if ((int)((CommerceModelSearch)(queryOperation.SearchCriteria)).Model.Properties["BasketType"] == 1) // when retrieving purchase order, supplement data that isn't auto mapped
            {
                foreach (CommerceEntity e in response.CommerceEntities) {
                    PurchaseOrder po = (operationCache.FirstOrDefault().Value as Dictionary<string, OrderGroup>)[e.Id] as PurchaseOrder;

                    bool skip = true;
                    foreach (var prop in e.Properties) {
                        if (skip) {
                            if (prop.Key == "BranchId") // only start setting custom properties
                                skip = false;
                            if (skip)
                                continue;
                        }
                        if (po[prop.Key] != null)
                            prop.Value = po[prop.Key];
                    }
                    // next do line items
                    if (e.Properties["LineItems"] != null && po.OrderForms.Count > 0) {
                        var orderForm = po.OrderForms[0];

                        if (orderForm.LineItems.Count > 0) {
                            LineItem[] lineItems = new LineItem[orderForm.LineItems.Count];
                            orderForm.LineItems.CopyTo(lineItems, 0);

                            foreach (var l in (e.Properties["LineItems"] as CommerceRelationshipList)) {
                                List<CommercePropertyItem> items = l.Target.Properties.Where(x => x.Value == null).ToList();
                                LineItem poLineItem = lineItems.Where(x => x.LineItemId.ToCommerceServerFormat() == l.Target.Id).FirstOrDefault();

                                skip = true;
                                foreach (var prop in l.Target.Properties) {
                                    if (skip) {
                                        if (prop.Key == "LinePosition") { skip = false; }// only lookup BEK custom properties
                                        if (skip) { continue; }
                                    } // end if(skip)

                                    if (poLineItem[prop.Key] != null) { prop.Value = poLineItem[prop.Key]; }
                                } //end foreach (l.Target.Properties)
                            } // end foreach(LineItems)

                        } // end if (PO's line item count > 0)
                    } // end if LineItems != null
                }
            }
        }
        #endregion

        #region properties
        protected virtual string ModelName {
            get {
                return "Basket";
            }
        }
        #endregion


    }
}