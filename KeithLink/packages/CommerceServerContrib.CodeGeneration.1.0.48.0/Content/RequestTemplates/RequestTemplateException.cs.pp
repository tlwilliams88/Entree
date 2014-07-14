//------------------------------------------------------------------------------
// <copyright file="RequestTemplateException.cs" company="CommerceServer.net, Inc">
//    (c) 2012 CommerceServer.net, Inc. and its affiliates. All rights reserved.
// </copyright>
// <summary></summary>
//------------------------------------------------------------------------------ 
namespace $rootnamespace$.RequestTemplates
{
	using System;
    using System.Collections.Generic;
    using System.Runtime.Serialization;
    using System.Security;
    using CommerceServer.Foundation.RequestTemplates;
	using System.Text;

	/// <summary>
    /// Exception which can be used to return Commerce Server Request Template errors. 
    /// </summary>
    [Serializable]
    public class RequestTemplateException : Exception
    {
		/// <summary>
        /// Initializes a new instance of the <see cref="RequestTemplateException" /> class.
        /// </summary>
        public RequestTemplateException() : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RequestTemplateException" /> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="innerException">The inner exception.</param>
        public RequestTemplateException(string message, Exception innerException) : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RequestTemplateException" /> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public RequestTemplateException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RequestTemplateException" /> class.
        /// </summary>
        /// <param name="info">The info.</param>
        /// <param name="context">The context.</param>
        protected RequestTemplateException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

		/// <summary>
		/// Gets the collection of Service Adapter errors returned by Commerce Server.
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Will causes a CA error until the Request Templates are generated.")]
        public List<ServiceAdapterErrorInfo> Errors { get; internal set; }

        /// <summary>
        /// Returns any errors in the exception
        /// </summary>
        public override string Message
        {
            get
            {
                var sb = new StringBuilder();

                sb.Append(base.Message);

                if (this.Errors != null && this.Errors.Count > 0)
                {
                    foreach (ServiceAdapterErrorInfo error in this.Errors)
                    {
                        sb.Append("\n\r" + error.ErrorMessage);
                    }
                }

                return sb.ToString();
            }
        }

		/// <summary>
        /// When overridden in a derived class, sets the <see cref="T:System.Runtime.Serialization.SerializationInfo"/> with information about the exception.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"/> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext"/> that contains contextual information about the source or destination.</param>
        /// <exception cref="T:System.ArgumentNullException">
        /// The <paramref name="info"/> parameter is a null reference (Nothing in Visual Basic).
        ///   </exception>
        ///   
        /// <PermissionSet>
        ///   <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Read="*AllFiles*" PathDiscovery="*AllFiles*"/>
        ///   <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="SerializationFormatter"/>
        /// </PermissionSet>
        [SecurityCritical]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
			info.AddValue("Errors", Errors);
        }
    }
}