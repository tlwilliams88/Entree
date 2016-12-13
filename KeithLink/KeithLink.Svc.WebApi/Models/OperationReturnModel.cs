using System;
using System.Collections.Generic;
using System.Text;

namespace KeithLink.Svc.WebApi.Models
{
    /// <summary>
    /// 
    /// </summary>
    public class OperationReturnModel<T>
    {
        /// <summary>
        /// 
        /// </summary>
        public string ErrorMessage { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public bool IsSuccess { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public T SuccessResponse { get; set; }
    }
}