using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CommerceServer.Core.Runtime.Orders;
using CommerceServer.Core.Runtime.Configuration;
using CommerceServer.Core.Catalog.XmlData;
using CommerceServer.Core.Orders;
using CommerceServer.Core.Runtime;
using CommerceServer.Core.Runtime.Pipelines;
using CommerceServer.Core.Interop.Caching;
using CommerceServer.Core.Interop;
using CommerceServer.Core.Interop.Targeting;
using CommerceServer.Core.Internal.Marketing;
using CommerceServer.Core.Catalog;
using System.Data;
using System.Collections;
using System.Data.Common;
using System.Runtime.InteropServices;
using System.Text;
using System.Globalization;

namespace KeithLink.Svc.FoundationSvc
{
	public class PipelineHelper
	{
		PipelineInfo basketPipe, totalPipe, checkoutPipe;
		CommerceResourceCollection resources;
		bool catContextCreated = false;
		String webSiteName;
		CommerceServer.Core.Catalog.CatalogContext catContext;
		OrderContext orderCtx;

		public PipelineHelper(String siteName, OrderContext orderContext)
		{
			resources = new CommerceResourceCollection(siteName);
			webSiteName = siteName;
			orderCtx = orderContext;
		}

		public String SiteName
		{
			get
			{
				return webSiteName;
			}

			set
			{
				webSiteName = (String)value;
			}
		}

		public void PopulateCart(ref Basket cart)
		{
			//Add an Address
			OrderAddress addr = new OrderAddress("HomeAddress", Guid.NewGuid().ToString());
			cart.Addresses.Add(addr);

			//Create and add an OrderForm
			cart.OrderForms.Add(new OrderForm("MainOrderForm"));

			//Add a LineItem from Adventure Works - we are assuming that is present and has Inventory present
			LineItem buyItem = new LineItem("Adventure Works Catalog", "AW078-04", "6", 1);
			buyItem.ShippingMethodId = GetShippingMethodID();
			cart.OrderForms[0].LineItems.Add(buyItem);

			//Add a dummy CreditCard Payment
			CreditCardPayment ccPmt = new CreditCardPayment(cart.Addresses[0].OrderAddressId, GetPaymentMethodID());
			ccPmt.CreditCardIdentifier = ccPmt.PaymentId.ToString();
			ccPmt.ExpirationMonth = 3;
			ccPmt.ExpirationYear = DateTime.Today.Year + 3;
		}

		//Assuming at least one CreditCard PaymentMethod has been created using the Orders UI
		public Guid GetPaymentMethodID()
		{
			Guid creditCardPaymentMethodId = Guid.Empty;
			Boolean ccPmtMethodFound = false;

			//Get all PaymentMethods 
			DataSet pmtMethods = orderCtx.GetPaymentMethods();
			DataTableReader reader = pmtMethods.CreateDataReader();

			IEnumerator readerEnum = reader.GetEnumerator();
			while (readerEnum.MoveNext() && !ccPmtMethodFound)
			{
				DbDataRecord rec = (DbDataRecord)readerEnum.Current;
				switch (rec.GetInt32(rec.GetOrdinal("PaymentType")))
				{
					case (int)PaymentMethodTypes.CreditCard:
						creditCardPaymentMethodId = rec.GetGuid(rec.GetOrdinal("PaymentMethodId"));
						ccPmtMethodFound = true;
						break;
				}
			}

			if (Guid.Empty == creditCardPaymentMethodId)
				throw new ApplicationException("This demo requires at least one CreditCard type of PaymentMethod to be defined!");

			return creditCardPaymentMethodId;
		}

		//Assuming at least one ShippingMethod has been created using the Orders UI
		public Guid GetShippingMethodID()
		{
			Guid shippingMethodId = Guid.Empty;
			Boolean shipMethodFound = false;

			//Get all PaymentMethods in default language - Hopefully there is one of each type
			DataSet shipMethods = orderCtx.GetShippingMethods();
			DataTableReader reader = shipMethods.CreateDataReader();

			IEnumerator readerEnum = reader.GetEnumerator();
			while (readerEnum.MoveNext() && !shipMethodFound)
			{
				DbDataRecord rec = (DbDataRecord)readerEnum.Current;
				shippingMethodId = rec.GetGuid(rec.GetOrdinal("ShippingMethodId"));
				shipMethodFound = true;
			}

			if (Guid.Empty == shippingMethodId)
				throw new ApplicationException("This demo requires at least one ShippingMethod to be defined!");

			return shippingMethodId;
		}

		public void CheckPipelineResultAndPrintWarnings(OrderGroup og, PipelineExecutionResult res)
		{
			//Check the error collections
			if (res == PipelineExecutionResult.Warning)
			{
				Console.WriteLine("Some warnings after running Pipelines on OrderGroup!\n");
				foreach (OrderForm of in og.OrderForms)
				{
					Console.WriteLine("_Basket_Errors Collection for OrderForm " + og.OrderForms.IndexOf(of) + ": ");
					foreach (String basketError in (SimpleList)of["_Basket_Errors"])
						Console.WriteLine("\t" + basketError);

					Console.WriteLine("_Purchase_Errors Collection for OrderForm " + og.OrderForms.IndexOf(of) + ": ");
					foreach (String poError in (SimpleList)of["_Purchase_Errors"])
						Console.WriteLine("\t" + poError);
				}
				Console.WriteLine();
			}
		}

		public PipelineExecutionResult RunPipeline(OrderGroup og, bool transacted, bool loggingEnabled, String pipelineName, String pipelinePath)
		{
			PipelineBase pipeline;
			PipelineExecutionResult res;
			PipelineInfo pipeInfo = CreatePipelineInfo(pipelineName);

			pipeline = new CommerceServer.Core.Runtime.Pipelines.OrderPipeline(pipelineName, pipelinePath, loggingEnabled, pipelineName + ".log", transacted);
			res = og.RunPipeline(pipeInfo, pipeline);

			CheckPipelineResultAndPrintWarnings(og, res);

			//Calling dispose on PipelineInfo to release any unmanaged resources
			pipeInfo.Dispose();

			return res;
		}

		public PipelineInfo CreatePipelineInfo(String pipelineName)
		{
			PipelineInfo pipeInfo;

			switch (pipelineName.ToLower())
			{
				case "basket":
					if (null == basketPipe)
					{
						basketPipe = new PipelineInfo(pipelineName);
						AddDataToPipelineInfo(ref basketPipe);
					}
					return basketPipe;
				case "total":
					if (null == totalPipe)
					{
						totalPipe = new PipelineInfo(pipelineName);
						AddDataToPipelineInfo(ref totalPipe);
					}
					return totalPipe;
				case "checkout":
					if (null == checkoutPipe)
					{
						checkoutPipe = new PipelineInfo(pipelineName);
						AddDataToPipelineInfo(ref checkoutPipe);
					}
					return checkoutPipe;
				default:
					pipeInfo = new PipelineInfo(pipelineName);
					AddDataToPipelineInfo(ref pipeInfo);
					return pipeInfo;
			}
		}

		private void AddDataToPipelineInfo(ref PipelineInfo pipeInfo)
		{
			CacheManager cacheManager = null;
			MessageManager msgManager = null;

			//Need to add Payment Processor Pipeline collection
			PipelineBase creditCardPipelineProcessor = new CommerceServer.Core.Runtime.Pipelines.OrderPipeline("CreditCardProcessor", Constants.creditCardPaymentProcessortPcfFilePath, false, Constants.LogFilePath + "\\CreditCard.log", true);

			PipelineCollection pipeCollection = new PipelineCollection();
			pipeCollection.Add("CreditCardProcessor", creditCardPipelineProcessor);

			pipeInfo["pipelines"] = pipeCollection;

			try
			{
				//Create the Cachemanager object
				cacheManager = new CacheManager();

				//Create the Messagemanager object
				msgManager = new MessageManager();

				AddMessagesToMessageManager(msgManager, CultureInfo.CurrentUICulture.ToString(), 1033);
				AddMessagesToMessageManager(msgManager, "en-US", 1033);

				//Set the components in the dictionary                
				ConfigureMarketingSystem(pipeInfo, cacheManager);
				ConfigureOrderSystem(pipeInfo, cacheManager);
				ConfigureCatalogSystem(pipeInfo);

				pipeInfo["MessageManager"] = msgManager;
				pipeInfo["CommerceResources"] = resources;

				pipeInfo["cachemanager"] = cacheManager;

				//Need to explicitly specify the Discount cache name when running in non-ASP.Net environment
				pipeInfo.DiscountsCacheName = "Discounts";
			}
			finally
			{
				if (cacheManager != null)
					Marshal.ReleaseComObject(cacheManager);
				if (msgManager != null)
					Marshal.ReleaseComObject(msgManager);
			}
		}

		public void AddMessagesToMessageManager(MessageManager msgMgr, String language, int locale)
		{
			msgMgr.AddLanguage(language, locale);
			msgMgr.AddMessage("pur_badsku", "One or more items were removed from the basket", language);
			msgMgr.AddMessage("pur_noitems", "No items were found in the basket", language);
			msgMgr.AddMessage("pur_badplacedprice", "Placed price was not correct", language);
			msgMgr.AddMessage("pur_discount_changed", "One or more discounts have changed", language);
			msgMgr.AddMessage("pur_discount_removed", "One or more discounts no longer apply", language);
			msgMgr.AddMessage("pur_badshipping", "Unable to complete order: cannot compute shipping cost", language);
			msgMgr.AddMessage("pur_badhandling", "Unable to complete order: cannot compute handling cost", language);
			msgMgr.AddMessage("pur_badtax", "Unable to complete order: cannot compute tax", language);
			msgMgr.AddMessage("pur_badcc", "The credit-card number you provided is not valid. Please verify your payment information or use a different card", language);
			msgMgr.AddMessage("pur_badpayment", "There was a problem authorizing your credit. Please verify your payment information or use a different card", language);
			msgMgr.AddMessage("pur_badverify", "Changes to the data require your review. Please review and re-submit", language);
			msgMgr.AddMessage("pur_out_of_stock", "At least one item is out of stock", language);
			msgMgr.AddMessage("unknown_shipping_method", "The selected shipping method is not currently available.  Please choose another shipping method", language);
		}

		void ConfigureMarketingSystem(PipelineInfo pipeInfo, CacheManager cacheManager)
		{
			CommerceResource marketingResource = resources["Marketing"];

			string marketingConnStr = marketingResource["connstr_db_marketing"].ToString();

			string cleanedMarketingConnStr;
			CleanSqlClientConnectionString(marketingConnStr, out cleanedMarketingConnStr);

			CommerceServer.Core.Runtime.IDictionary marketingCacheDict = new Dictionary();
			marketingCacheDict["ConnectionString"] = marketingConnStr;
			marketingCacheDict["DefaultLanguage"] = marketingResource["s_DefaultMarketingSystemLanguage"].ToString();

			ExpressionEvaluator exprEval = null;

			try
			{
				//Create the expression evaluator
				IExpressionStoreAdapter evaluatorStoreAdapter = new MarketingExpressionStoreAdapter(cleanedMarketingConnStr);
				exprEval = new ExpressionEvaluator();
				exprEval.Connect(evaluatorStoreAdapter);

				//Set Discounts cache.
				cacheManager.set_RefreshInterval("Discounts", 0);
				cacheManager.set_RetryInterval("Discounts", 60);
				cacheManager.set_CacheObjectProgId("Discounts", "Commerce.Dictionary");
				cacheManager.set_LoaderProgId("Discounts", "Commerce.CSFLoadDiscounts");
				marketingCacheDict["Evaluator"] = exprEval;
				cacheManager.set_LoaderConfig("Discounts", marketingCacheDict);
				cacheManager.set_WriterConfig("Discounts", marketingCacheDict);
				cacheManager.set_WriterProgId("Discounts", "Commerce.CSFWriteEvents");

				//Set Advertising cache.
				cacheManager.set_RefreshInterval("Advertising", 300);
				cacheManager.set_RetryInterval("Advertising", 60);
				cacheManager.set_CacheObjectProgId("Advertising", "Commerce.Dictionary");
				cacheManager.set_LoaderProgId("Advertising", "Commerce.CSFLoadAdvertisements");
				cacheManager.set_WriterConfig("Advertising", marketingCacheDict);
				cacheManager.set_WriterProgId("Advertising", "Commerce.CSFWriteEvents");

				pipeInfo["Evaluator"] = exprEval;
			}
			finally
			{
				if (exprEval != null)
					Marshal.ReleaseComObject(exprEval);
				if (marketingCacheDict != null)
					Marshal.ReleaseComObject(marketingCacheDict);
			}
		}

		void ConfigureOrderSystem(PipelineInfo pipeInfo, CacheManager cacheManager)
		{
			CommerceResource transactionConfigResource = resources["Transaction Config"];
			string transactionConfigConnStr = transactionConfigResource["connstr_db_TransactionConfig"].ToString();
			string cleanedTxConfigConnstr = null;
			CleanSqlClientConnectionString(transactionConfigConnStr, out cleanedTxConfigConnstr);

			CommerceServer.Core.Runtime.IDictionary shippingMethodCacheDict = new Dictionary();
			shippingMethodCacheDict["ConnectionString"] = cleanedTxConfigConnstr;
			shippingMethodCacheDict["TableName"] = "txVirtual_Directory";

			CommerceServer.Core.Runtime.IDictionary paymentMethodCacheDict = new Dictionary();
			paymentMethodCacheDict["ConnectionString"] = cleanedTxConfigConnstr;
			paymentMethodCacheDict["TableName"] = "txVirtual_Directory";

			pipeInfo["TransactionConfigConnectionString"] = transactionConfigConnStr;
			try
			{
				//Set shipping method cache.
				cacheManager.set_RefreshInterval("ShippingManagerCache", 0);
				cacheManager.set_RetryInterval("ShippingManagerCache", 30);
				cacheManager.set_CacheObjectProgId("ShippingManagerCache", "Commerce.Dictionary");
				cacheManager.set_LoaderProgId("ShippingManagerCache", "Commerce.ShippingMethodCache");
				cacheManager.set_LoaderConfig("ShippingManagerCache", shippingMethodCacheDict);
				cacheManager.set_WriterConfig("ShippingManagerCache", shippingMethodCacheDict);

				//Set payment method cache.
				cacheManager.set_RefreshInterval("PaymentMethodCache", 0);
				cacheManager.set_RetryInterval("PaymentMethodCache", 30);
				cacheManager.set_CacheObjectProgId("PaymentMethodCache", "Commerce.Dictionary");
				cacheManager.set_LoaderProgId("PaymentMethodCache", "Commerce.PaymentMethodCache");
				cacheManager.set_LoaderConfig("PaymentMethodCache", paymentMethodCacheDict);
				cacheManager.set_WriterConfig("PaymentMethodCache", paymentMethodCacheDict);
			}
			finally
			{
				if (shippingMethodCacheDict != null)
					Marshal.ReleaseComObject(shippingMethodCacheDict);
			}
		}

		void ConfigureCatalogSystem(PipelineInfo pipeInfo)
		{
			if (!catContextCreated)
			{
				CatalogSiteAgent siteAgent = new CatalogSiteAgent();
				siteAgent.SiteName = SiteName;

				//Create the catalog cache options
				CacheConfiguration catCacheConfiguration = new CacheConfiguration();
				catCacheConfiguration.SchemaCacheTimeout = catCacheConfiguration.ItemInformationCacheTimeout =
				catCacheConfiguration.ItemHierarchyCacheTimeout = catCacheConfiguration.ItemRelationshipsCacheTimeout =
				catCacheConfiguration.ItemAssociationsCacheTimeout = catCacheConfiguration.CatalogCollectionCacheTimeout =
				TimeSpan.FromMinutes(5);

				//Now create the CatalogContext with the cache configurations specified to setup the caching
				catContext = CommerceServer.Core.Catalog.CatalogContext.Create(siteAgent, catCacheConfiguration);
				catContextCreated = true;
			}
			pipeInfo["CatalogContext"] = catContext;
		}

		public bool CleanSqlClientConnectionString(string connectionString, out string cleaned)
		{
			const string PROVIDER_TOKEN = "Provider";
			const string PROVIDER_NAME = "SQLOLEDB";
			string[] stringArr;
			string[] tokenValues;
			cleaned = null;
			StringBuilder cleanedBuilder = new StringBuilder();

			// divide the connection string into multiple clauses, specified by ';'.
			stringArr = connectionString.Split(';');

			// loop through clauses to remove one or more Provider clauses.
			foreach (string strClause in stringArr)
			{
				tokenValues = strClause.Split('=');
				// is this a Provider clause?
				if (String.Compare(tokenValues[0].Trim(), PROVIDER_TOKEN, true, CultureInfo.InvariantCulture) == 0)
				{
					//not a valid SqlOleDB provider if the provider string is empty.
					if (tokenValues.Length < 2)
						return false;

					// not a valid SqlOleDB provider if the provider string is not SQLOLEDB or SQLOLEDB.X (X is the version number)
					if (String.Compare(tokenValues[1].Trim(), 0, PROVIDER_NAME, 0, PROVIDER_NAME.Length, true,
						CultureInfo.InvariantCulture) != 0)
					{
						return false;
					}
				}
				else
				{
					if (strClause.Length != 0)
					{
						cleanedBuilder.Append(strClause);
						cleanedBuilder.Append(';');
					}
				}
			}

			cleaned = cleanedBuilder.ToString();
			return true;
		}
	}
	
}