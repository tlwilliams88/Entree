﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using Newtonsoft.Json.Serialization;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using KeithLink.Svc.Core.Interface.ModelExport;
using KeithLink.Svc.Core.Models.ModelExport;
using System.ComponentModel;

namespace KeithLink.Svc.Core.Models.SiteCatalog
{
    [DataContract(Name = "product")]
    [Serializable]
    public class Product : BaseProductInfo, IExportableModel
    {
        #region ctor
        public Product()
        {
            ProductImages = new List<ProductImage>();
            OrderHistory = new Dictionary<string, int>();
        }
        #endregion

        #region methods
        public List<ExportModelConfiguration> DefaultExportConfiguration()
        {
            var defaultConfig = new List<ExportModelConfiguration>();

            defaultConfig.Add(new ExportModelConfiguration() { Field = "ItemNumber", Order = 1, Label = "Item" });
            defaultConfig.Add(new ExportModelConfiguration() { Field = "Name", Order = 10, Label = "Name" });
            defaultConfig.Add(new ExportModelConfiguration() { Field = "BrandExtendedDescription", Order = 20, Label = "Brand" });
            defaultConfig.Add(new ExportModelConfiguration() { Field = "Pack", Order = 30, Label = "Pack" });
            defaultConfig.Add(new ExportModelConfiguration() { Field = "Size", Order = 40, Label = "Size" });
            defaultConfig.Add(new ExportModelConfiguration() { Field = "CaseOnly", Order = 50, Label = "Each" });
            defaultConfig.Add(new ExportModelConfiguration() { Field = "UnitCost", Order = 60, Label = "Cost" });
            defaultConfig.Add(new ExportModelConfiguration() { Field = "CasePrice", Order = 70, Label = "Price" });


            return defaultConfig;
        }

        [OnSerializing]
        void OnSerializing(StreamingContext context)
        {
            //Do not output a "blank" upc
            if (this.UPC != null)
            {
                if (this.UPC.Equals("00000000000000"))
                    this.UPC = string.Empty;
            }

        }
        #endregion

        #region properties
        [DataMember(Name = "ext_description")]
        public string ExtendedDescription { get; set; }

        [DataMember(Name = "cube")]
        public string CaseCube { get; set; }

        [DataMember(Name = "productimages")]
        public List<ProductImage> ProductImages { get; set; }

        [DataMember(Name = "isproprietary")]
        public bool IsProprietary { get; set; }

        [DataMember(Name = "orderhistory")]
        public Dictionary<string, int> OrderHistory { get; set; }

        [DataMember(Name = "inhistory")]
        public bool InHistory { get; set; }
        #endregion
    }


}