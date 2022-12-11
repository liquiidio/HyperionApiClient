using HyperionApiClient.Core;
using HyperionApiClient.Core.Clients;

namespace HyperionApiClient.Clients
{
    public class StatsClient : StatsClientBase
    {
        public StatsClient(IHttpHandler httpHandler) : base(httpHandler)
        {

        }
    }
}