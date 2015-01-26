using KeithLink.Utility.BEKWarmup.Models;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Utility.BEKWarmup
{
	class Program
	{
		static void Main(string[] args)
		{
			try
			{
				DateTime overallStartTime = DateTime.Now;
				string apiUrl = ConfigurationManager.AppSettings.Get("APIUrl");
				string apiKey = ConfigurationManager.AppSettings.Get("APIKey");
				string userName = ConfigurationManager.AppSettings.Get("userName");
				string userPassword = ConfigurationManager.AppSettings.Get("userPassword");

				var client = new RestSharp.RestClient(apiUrl);
				client.ReadWriteTimeout = 60000;

				//Authenticate
				var authRequest = new RestRequest("authen", Method.POST);
				authRequest.AddParameter("grant_type", "password");
				authRequest.AddParameter("username", userName);
				authRequest.AddParameter("password", userPassword);
				authRequest.AddParameter("apiKey", apiKey);
				authRequest.AddHeader("Content-Type", "application/x-www-form-urlencoded");
				authRequest.AddHeader("Accept", "application/json");
				var authResponse = client.Execute<AuthenResponse>(authRequest);

				//Get User Profile
				var profileRequest = new RestRequest("/profile?email=" + userName, Method.GET);
				profileRequest.AddHeader("Authorization", string.Format("Bearer {0}", authResponse.Data.access_token));
				profileRequest.AddHeader("apiKey", ConfigurationManager.AppSettings.Get("APIKey"));
				var profileResponse = client.Execute<GetProfileResponse>(profileRequest);

				var customerRequest = new RestRequest("/profile/customer?size=15&from=0", Method.GET);
				customerRequest.AddHeader("Authorization", string.Format("Bearer {0}", authResponse.Data.access_token));
				customerRequest.AddHeader("apiKey", ConfigurationManager.AppSettings.Get("APIKey"));
				var customerResponse = client.Execute<PagedCustomerResponse>(customerRequest);


				Dictionary<string, TimeSpan> timePerCustomer = new Dictionary<string, TimeSpan>();
				foreach (var customer in customerResponse.Data.results)
				{
					DateTime startTime = DateTime.Now;
					// get user lists
					var listRequest = new RestRequest("/list", Method.GET);
					var listResponse = MakeGetRequest(client, authResponse.Data.access_token, listRequest, customer.customerNumber, customer.customerBranch);

					// get user carts
					var cartRequest = new RestRequest("/cart", Method.GET);
					var cartResponse = MakeGetRequest(client, authResponse.Data.access_token, cartRequest, customer.customerNumber, customer.customerBranch);

					// change orders
					var changeOrderRequest = new RestRequest("/order/changeorder", Method.GET);
					var changeOrderResponse = MakeGetRequest(client, authResponse.Data.access_token, changeOrderRequest, customer.customerNumber, customer.customerBranch);
					timePerCustomer.Add(customer.customerNumber, DateTime.Now - startTime);
				}

				Console.WriteLine("Overall run time: " + (DateTime.Now - overallStartTime).ToString());
				foreach (var entry in timePerCustomer)
				{
					Console.WriteLine("Customer " + entry.Key + " took " + entry.Value.ToString());
				}
			}
			catch (Exception ex)
			{
				string sSource = "BEK_Shop";
				string sLog = "Application";
				string sEvent = "BEK Exception - WarmUpScript - " + ex.Message + ": " + ex.StackTrace;

				if (!System.Diagnostics.EventLog.SourceExists(sSource))
					System.Diagnostics.EventLog.CreateEventSource(sSource, sLog);
				System.Diagnostics.EventLog.WriteEntry(sSource, sEvent, System.Diagnostics.EventLogEntryType.Warning, 234);
			}

		}

		private static IRestResponse MakeGetRequest(RestClient client, string accessToken, RestRequest request, string customerNum, string branchId)
		{
			request.AddHeader("Authorization", string.Format("Bearer {0}", accessToken));
			request.AddHeader("apiKey", ConfigurationManager.AppSettings.Get("APIKey"));
			request.AddHeader("userSelectedContext", @"{""customerid"":""" + customerNum + @""",""branchid"":""" + branchId + @"""}");
			return client.Execute(request);
		}
	}
}
