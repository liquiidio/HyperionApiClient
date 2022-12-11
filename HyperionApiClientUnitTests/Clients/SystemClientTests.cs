using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading.Tasks;
using HyperionApiClient.Clients;
using HyperionApiClient;

namespace EosRio.HyperionApi.Tests
{
    [TestClass()]
    public class SystemClientTests
    {
        [TestMethod()]
        public async Task SystemClientTest()
        {
            var systemClient = new SystemClient(new HttpClientHandler())
            {
                BaseUrl = "invalidUrl"
            };

            try
            {
                var acc = await systemClient.GetVotersAsync();
                Assert.Fail();
            }
            catch (Exception)
            {
                Assert.IsTrue(true);
            }
        }

        [TestMethod()]
        public async Task GetProposalsAsyncTest()
        {
            var systemClient = new SystemClient(new HttpClientHandler());

            var proposalsResponse = await systemClient.GetProposalsAsync();

            Assert.IsNotNull(proposalsResponse.Total);
            Assert.IsNotNull(proposalsResponse.Proposals);
        }

        [TestMethod()]
        public async Task GetVotersAsyncTest()
        {
            var systemClient = new SystemClient(new HttpClientHandler());

            var votersResponse = await systemClient.GetVotersAsync();

            Assert.IsNotNull(votersResponse.Voters);
        }
    }
}