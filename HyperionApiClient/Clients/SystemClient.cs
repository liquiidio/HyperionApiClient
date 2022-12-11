using HyperionApiClient.Core;
using HyperionApiClient.Core.Clients;

namespace HyperionApiClient.Clients
{
    public class SystemClient : SystemClientBase
    {
        public SystemClient(IHttpHandler httpHandler) : base(httpHandler)
        {

        }
    }
}