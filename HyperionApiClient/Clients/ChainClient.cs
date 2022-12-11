using HyperionApiClient.Core;
using HyperionApiClient.Core.Clients;

namespace HyperionApiClient.Clients
{
    public class ChainClient : ChainClientBase
    {
        public ChainClient(IHttpHandler httpHandler) : base(httpHandler)
        {

        }
    }
}