using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace KeithLink.Svc.Core.Models.SpecialOrders.EF
{
	public class RequestHeaderId
	{
        [Key, Required]
        public string CurrentId { get; set; }
    }
}
