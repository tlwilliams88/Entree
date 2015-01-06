using KeithLink.Ext.Pipeline.ItemPrice.PipelineService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Ext.Pipeline.ItemPrice
{
	public class PipelineServiceHelper
	{
		internal static PipelineServiceClient CreateWebServiceInstance(string url)
		{
			BasicHttpBinding binding = new BasicHttpBinding();
			// I think most (or all) of these are defaults--I just copied them from app.config:
			binding.SendTimeout = TimeSpan.FromMinutes(1);
			binding.OpenTimeout = TimeSpan.FromMinutes(1);
			binding.CloseTimeout = TimeSpan.FromMinutes(1);
			binding.ReceiveTimeout = TimeSpan.FromMinutes(10);
			binding.MaxReceivedMessageSize = int.MaxValue;
			binding.MaxBufferPoolSize = int.MaxValue;
			binding.MaxBufferPoolSize = int.MaxValue;
			binding.AllowCookies = false;
			binding.BypassProxyOnLocal = false;
			binding.HostNameComparisonMode = HostNameComparisonMode.StrongWildcard;
			binding.MessageEncoding = WSMessageEncoding.Text;
			binding.TextEncoding = System.Text.Encoding.UTF8;
			binding.TransferMode = TransferMode.Buffered;
			binding.UseDefaultWebProxy = true;
			return new PipelineServiceClient(binding, new EndpointAddress(url));
		}
	}
}
