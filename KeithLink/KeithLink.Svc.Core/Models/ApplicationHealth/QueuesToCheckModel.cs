﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Core.Models.ApplicationHealth
{
    public class QueuesToCheckModel
    {
        public List<QueueToCheckModel> targets { get; set; }
    }
}
