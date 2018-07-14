﻿using System;
using System.Xml.Serialization;

namespace Entree.Core.SiteCatalog.Models.Pricing.PowerMenu {
    [XmlRoot("Envelope", Namespace = "http://schemas.xmlsoap.org/soap/envelope/")]
    public class SoapEnvelope {
        #region ctor
        public SoapEnvelope() {
            Body = new SoapBody();
        }
        #endregion

        #region properties
        [XmlElement(Namespace = "http://schemas.xmlsoap.org/soap/envelope/")]
        public SoapBody Body { get; set; }
        #endregion
    }
}
