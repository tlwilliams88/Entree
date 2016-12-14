using System;
using System.Collections.Generic;
using System.Text;

namespace KeithLink.Svc.WebApi.Models
{
    /// <summary>
    /// OperationReturnModel
    /// </summary>
    public class OperationReturnModel<T>
    {
        /// <summary>
        /// ErrorMessage
        /// </summary>
        public string ErrorMessage { get; set; }
        /// <summary>
        /// IsSuccess
        /// </summary>
        public bool IsSuccess { get; set; }
        /// <summary>
        /// SuccessResponse
        /// </summary>
        public T SuccessResponse { get; set; }
    }
}