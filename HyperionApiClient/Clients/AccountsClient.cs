using HyperionApiClient.Core;
using HyperionApiClient.Core.Clients;

namespace HyperionApiClient.Clients
{
    public class AccountsClient : AccountsClientBase
    {
        public AccountsClient(IHttpHandler httpHandler) : base(httpHandler)
        {

        }
    }
}