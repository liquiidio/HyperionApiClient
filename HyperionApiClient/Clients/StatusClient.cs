using HyperionApiClient.Core;
using HyperionApiClient.Core.Clients;

namespace HyperionApiClient.Clients
{
    public class StatusClient : StatusClientBase
    {
        public StatusClient(IHttpHandler httpHandler) : base(httpHandler)
        {

        }
    }
}