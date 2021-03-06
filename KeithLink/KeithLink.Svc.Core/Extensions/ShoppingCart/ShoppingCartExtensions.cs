﻿// KeithLink
using KeithLink.Common.Core.Extensions;

using KeithLink.Svc.Core.Helpers;
using KeithLink.Svc.Core.Models.Lists;
using KeithLink.Svc.Core.Models.ShoppingCart;


// Core
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Core.Extensions.ShoppingCart {
    public static class ShoppingCartItemExtensions {

        public static List<ShoppingCartItemReportModel> ToReportModel( this List<ShoppingCartItem> items ) {
            List<ShoppingCartItemReportModel> newList = new List<ShoppingCartItemReportModel>();

            foreach (ShoppingCartItem i in items) {
                ShoppingCartItemReportModel n = new ShoppingCartItemReportModel();

                n.ItemNumber = i.ItemNumber;
                n.Name = i.Name;
                n.Brand = i.Brand;
                n.Category = i.ItemClass;
                n.PackSize = i.PackSize;
                if (i.Notes != null) {
                    n.Notes = i.Notes;
                }
                n.Label = i.Label;
                n.ParLevel = i.ParLevel;
                n.Quantity = i.Quantity;
                n.Each = i.Each;
                n.CasePrice = i.CasePrice.ToDouble().Value;
                n.PackagePrice = i.PackagePrice.ToDouble().Value;

                n.ExtPrice = PricingHelper.GetPrice((int)i.Quantity, i.CasePriceNumeric, i.PackagePriceNumeric,
                                                    i.Each, i.CatchWeight, i.AverageWeight,
                                                    i.Pack.ToInt(1));

                newList.Add( n );
            }


            return newList;

        }
    }
}
