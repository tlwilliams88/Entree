﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace KeithLink.Svc.Core.Models.ContentManagement
{
    [DataContract]
    public class ContentItemViewModel : ContentItemModelBase
    {
        [DataMember(Name="imageurl")]
        public string ImageUrl { get; set; }
    }
}
