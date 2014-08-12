using System;
using System.Collections.Generic;
using System.Text;

namespace KeithLink.Svc.WebApi.Models
{
    public class OperationReturnModel<T>
    {
        public string ErrorMessage { get; set; }

        public T SuccessResponse { get; set; }
    }
}