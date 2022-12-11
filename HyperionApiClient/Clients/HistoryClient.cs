using HyperionApiClient.Core;
using HyperionApiClient.Core.Clients;

namespace HyperionApiClient.Clients
{
    public class HistoryClient : HistoryClientBase
    {
        public HistoryClient(IHttpHandler httpHandler) : base(httpHandler)
        {

        }
    }
}