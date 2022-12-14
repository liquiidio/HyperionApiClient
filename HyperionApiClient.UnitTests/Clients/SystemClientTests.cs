using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading.Tasks;
using HyperionApiClient.Clients;

namespace HyperionApiClient.UnitTests.Clients
{
    [TestClass()]
    public class SystemClientTests
    {
        [TestMethod()]
        public async Task SystemClientTest()
        {
            var systemClient = new SystemClient(new HttpHandler())
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
            var systemClient = new SystemClient(new HttpHandler());

            var proposalsResponse = await systemClient.GetProposalsAsync();

            Assert.IsNotNull(proposalsResponse.Total);
            Assert.IsNotNull(proposalsResponse.Proposals);
        }

        [TestMethod()]
        public async Task GetVotersAsyncTest()
        {
            var systemClient = new SystemClient(new HttpHandler());

            var votersResponse = await systemClient.GetVotersAsync();

            Assert.IsNotNull(votersResponse.Voters);
        }
    }
}