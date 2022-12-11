using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading.Tasks;
using HyperionApiClient.Clients;
using HyperionApiClient;

namespace EosRio.HyperionApi.Tests
{
    [TestClass()]
    public class StatsClientTests
    {
        [TestMethod()]
        public async Task StatsClientTest()
        {
            var statsClient = new StatsClient(new HttpClientHandler())
            {
                BaseUrl = "invalidUrl"
            };

            try
            {
                var resourceUsageResponse = await statsClient.GetResourceUsageAsync("eosio.token","transfer");
                Assert.Fail();
            }
            catch (Exception)
            {
                Assert.IsTrue(true);
            }
        }

        [TestMethod()]
        public async Task GetActionUsageAsyncTest()
        {
            var statsClient = new StatsClient(new HttpClientHandler());

            var actionUsageResponse = await statsClient.GetActionUsageAsync("1h");

            Assert.IsNotNull(actionUsageResponse.From);
            Assert.IsNotNull(actionUsageResponse.Period);
            Assert.IsNotNull(actionUsageResponse.To);
        }

        [TestMethod()]
        public async Task GetMissedBlocksAsyncTest()
        {
            var statsClient = new StatsClient(new HttpClientHandler());

            var missedBlocksResponse = await statsClient.GetMissedBlocksAsync("liquidstudios");

            Assert.IsNotNull(missedBlocksResponse.Stats);
            Assert.IsNotNull(missedBlocksResponse.Lib);
        }

        [TestMethod()]
        public async Task GetResourceUsageAsyncTest()
        {
            var statsClient = new StatsClient(new HttpClientHandler());

            var resourceUsageResponse = await statsClient.GetResourceUsageAsync("eosio.token", "transfer");

            Assert.IsNotNull(resourceUsageResponse.Cpu);
            Assert.IsNotNull(resourceUsageResponse.Net);

            Assert.IsNotNull(resourceUsageResponse.Cpu.Stats);
            Assert.IsNotNull(resourceUsageResponse.Net.Stats);
        }
    }
}