﻿using KeithLink.Common.Core.Extensions;

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
       public override void ExecuteQuery(CommerceQueryOperation queryOperation, OperationCacheDictionary operationCache, CommerceQueryOperationResponse response)
        {
            if ((int)((CommerceServer.Foundation.CommerceModelSearch)(queryOperation.SearchCriteria)).Model.Properties["BasketType"] == 1) // when retrieving purchase order, supplement data that isn't auto mapped
            {
                foreach (CommerceEntity e in response.CommerceEntities)
                {
                    CommerceServer.Core.Runtime.Orders.PurchaseOrder po =
                        (operationCache.FirstOrDefault().Value as Dictionary<string, CommerceServer.Core.Runtime.Orders.OrderGroup>)[e.Id] as CommerceServer.Core.Runtime.Orders.PurchaseOrder;
                    CommerceServer.Core.Runtime.Orders.LineItem[] lineItems = new CommerceServer.Core.Runtime.Orders.LineItem[po.OrderForms[0].LineItems.Count];
 
                    if (po.OrderForms[0].LineItems.Count > 0)
                    {
                        po.OrderForms[0].LineItems.CopyTo(lineItems, 0);
                    }
 
                    bool skip = true;
                    foreach (var prop in e.Properties)
                    {
                        if (skip)
                        {
                            if (prop.Key == "BranchId") // only start setting custom properties
                                skip = false;
                            if (skip)
                                continue;
                        }
                        if (po[prop.Key] != null)
                            prop.Value = po[prop.Key];
                    }
                    // next do line items
                   if(e.Properties["LineItems"] != null)
                       foreach (var l in (e.Properties["LineItems"] as CommerceRelationshipList))
                       {
                           List<CommercePropertyItem> items = l.Target.Properties.Where(x => x.Value == null).ToList();
                           CommerceServer.Core.Runtime.Orders.LineItem poLineItem = lineItems.Where(x => x.LineItemId.ToCommerceServerFormat() == l.Target.Id).FirstOrDefault();
                           skip = true;
                           foreach (var prop in l.Target.Properties)
                           {
                               if (skip)
                               {
                                   if (prop.Key == "LinePosition") // only lookup BEK custom properties
                                       skip = false;
                                   if (skip)
                                       continue;
                                
                               }
                               if (poLineItem[prop.Key] != null)
                                   prop.Value = poLineItem[prop.Key];
                           }
                       }
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