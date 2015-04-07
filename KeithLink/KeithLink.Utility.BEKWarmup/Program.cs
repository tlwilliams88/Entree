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

				if (authResponse == null || authResponse.StatusCode != System.Net.HttpStatusCode.OK ||  authResponse.Data == null || authResponse.Data.access_token == null)
				{
					string sSource = "BEK_Shop";
					string sLog = "Application";
					string sEvent = "BEK Authentication Failure - WarmUpScript - " + authResponse.Content;

					if (!System.Diagnostics.EventLog.SourceExists(sSource))
						System.Diagnostics.EventLog.CreateEventSource(sSource, sLog);
					System.Diagnostics.EventLog.WriteEntry(sSource, sEvent, System.Diagnostics.EventLogEntryType.Warning, 234);
				}

				//Get User Profile
				var profileRequest = new RestRequest("/profile?email=" + userName, Method.GET);
				profileRequest.AddHeader("Authorization", string.Format("Bearer {0}", authResponse.Data.access_token));
				profileRequest.AddHeader("apiKey", ConfigurationManager.AppSettings.Get("APIKey"));
				var profileResponse = client.Execute<GetProfileResponse>(profileRequest);

				var customerRequest = new RestRequest("/profile/customer?account=&from=0&size=5&terms=", Method.GET);
				customerRequest.AddHeader("Authorization", string.Format("Bearer {0}", authResponse.Data.access_token));
				customerRequest.AddHeader("apiKey", ConfigurationManager.AppSettings.Get("APIKey"));
				var customerResponse = client.Execute<PagedCustomerResponse>(customerRequest);

				//Getting divisions is a lightweight query that will keep the internal service alive
				var divisionRequest = new RestRequest("/catalog/divisions", Method.GET);
				divisionRequest.AddHeader("apiKey", ConfigurationManager.AppSettings.Get("APIKey"));
				var divisionResponse = client.Execute(divisionRequest);
				
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
