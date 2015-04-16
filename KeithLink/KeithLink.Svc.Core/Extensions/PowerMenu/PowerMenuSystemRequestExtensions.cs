using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml.Serialization;
using System.Xml;
using KeithLink.Svc.Core.Models.PowerMenu;

namespace KeithLink.Svc.Core.Extensions.PowerMenu {
    public static class PowerMenuSystemRequestExtensions {

        public static string ToXML( this PowerMenuSystemRequestModel PowerMenuRequest ) {
            string returnValue = "";

            using (MemoryStream stream = new MemoryStream()) {
                using (XmlWriter writer = XmlWriter.Create( stream )) {
                    // Remove the xmlns namespaces from the xml responses
                    XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
                    ns.Add( "", "" );

                    new XmlSerializer( typeof( PowerMenuSystemRequestModel ), string.Empty ).Serialize( writer, PowerMenuRequest, ns );
                    returnValue = Encoding.UTF8.GetString( stream.ToArray() );
                }
            }

            if (returnValue.Length > 0) {
                return returnValue;
            } else {
                throw new Exception( String.Format( "Serialization failed for Power Menu: {0}", returnValue ) );
            }
        }

    }
}
