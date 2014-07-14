//------------------------------------------------------------------------------
// <copyright file="ControllerFactory.cs" company="CommerceServer.net, Inc">
//    (c) 2012 CommerceServer.net, Inc. and its affiliates. All rights reserved.
// </copyright>
// <summary></summary>
//------------------------------------------------------------------------------ 
namespace $rootnamespace$.RequestTemplates
{
	using CommerceServerContrib.Web.Controllers;
	using CommerceServerContrib.CrossTier.Interfaces.Controllers;

	/// <summary>
    /// A simple factory for returning the Commerce Server Request Template Factory based on Area.
    /// </summary>
	public static class ControllerFactory
	{
		/// <summary>
        /// Method which returns the right controller for the area. 
        /// </summary>
        /// <param name="area">Area (Catalog, Profiles, etc.) required.</param>
        /// <returns>Commerce Server Contrib Controller instance.</returns>
		public static BaseController<IRequestTemplateController> GetController(string area)
        {
            switch (area)
            {
                case "Orders":
                    return new OrdersController();
                case "Marketing":
                    return new MarketingController();
                default:
                    return new CatalogController();
            }
        }
	}
}