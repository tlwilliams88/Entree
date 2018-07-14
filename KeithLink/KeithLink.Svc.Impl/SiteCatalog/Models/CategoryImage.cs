using System.Runtime.Serialization;

namespace Entree.Core.SiteCatalog.Models
{
    [DataContract(Name="category_image")]
    public class CategoryImage
    {
        #region properties
        
        [DataMember(Name="filename")]
        public string FileName { get; set; }

        [DataMember(Name="url")]
        public string Url { get; set; }

        [DataMember(Name="width")]
        public string Width { get; set; }

        [DataMember(Name="height")]
        public string Height { get; set; }

        #endregion
    }
}
