using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading.Tasks;
using HyperionApiClient.Clients;

namespace HyperionApiClient.UnitTests.Clients
{
    [TestClass()]
    public class StatusClientTests
    {
        [TestMethod()]
        public async Task StatusClientTest()
        {
            var statusClient = new StatusClient(new HttpHandler())
            {
                BaseUrl = "invalidUrl"
            };

            try
            {
                var acc = await statusClient.HealthAsync();
                Assert.Fail();
            }
            catch (Exception)
            {
                Assert.IsTrue(true);
            }
        }

        [TestMethod()]
        public async Task HealthAsyncTest()
        {
            var statusClient = new StatusClient(new HttpHandler());

            var healthResponse = await statusClient.HealthAsync();
            Assert.IsNotNull(healthResponse.Features);
            Assert.IsNotNull(healthResponse.Health);
            Assert.IsNotNull(healthResponse.Host);
            Assert.IsNotNull(healthResponse.Version);
            Assert.IsNotNull(healthResponse.VersionHash);
        }
    }
}