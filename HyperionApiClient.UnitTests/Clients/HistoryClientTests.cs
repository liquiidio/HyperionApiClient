﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Threading.Tasks;
using HyperionApiClient.Clients;

namespace HyperionApiClient.UnitTests.Clients
{
    [TestClass()]
    public class HistoryClientTests
    {
        [TestMethod()]
        public async Task HistoryClientTest()
        {
            var historyClient = new HistoryClient(new HttpClientHandler())
            {
                BaseUrl = "invalidUrl"
            };

            try
            {
                var acc = await historyClient.GetBlockAsync();
                Assert.Fail();
            }
            catch (Exception)
            {
                Assert.IsTrue(true);
            }
        }

        [TestMethod()]
        public async Task GetAbiSnapshotAsyncTest()
        {
            var historyClient = new HistoryClient(new HttpClientHandler());

            var blockNum = 10000;
            var snapshotResponse = await historyClient.GetAbiSnapshotAsync("eosio.token", blockNum);

            Assert.IsTrue(snapshotResponse.BlockNum <= blockNum);
            Assert.IsNull(snapshotResponse.Abi);

            snapshotResponse = await historyClient.GetAbiSnapshotAsync("eosio.token", blockNum, true);
            Assert.IsNotNull(snapshotResponse.Abi);
            Assert.IsTrue(snapshotResponse.Abi.Actions.Any(a => a.Name == "transfer"));
            Assert.IsTrue(snapshotResponse.Abi.Structs.Any(a => a.Name == "account"));
            Assert.IsTrue(snapshotResponse.Abi.Tables.Any(a => a.Name == "accounts"));
        }

        [TestMethod()]
        public async Task GetActionsAsyncTest()
        {
            var historyClient = new HistoryClient(new HttpClientHandler());

            var actionsResponse = await historyClient.GetActionsAsync(null, null, "kingcoolcorv");
            Assert.IsNotNull(actionsResponse.Actions);
            Assert.IsNotNull(actionsResponse.Total);
            Assert.IsTrue(actionsResponse.Actions.All(a => a.Act.Name != null));
            Assert.IsTrue(actionsResponse.Actions.All(a => a.Producer != null));
            Assert.IsTrue(actionsResponse.Actions.All(a => a.TrxId != null));
        }

        [TestMethod()]
        public async Task GetDeltasAsyncTest()
        {
            var historyClient = new HistoryClient(new HttpClientHandler());

            var contract = "eosio.token";
            var deltasResponse = await historyClient.GetDeltasAsync(null, null, contract);
            Assert.IsNotNull(deltasResponse.Deltas);
            Assert.IsNotNull(deltasResponse.Total);
            Assert.IsTrue(deltasResponse.Deltas.All(d => d.Code != null));
            Assert.IsTrue(deltasResponse.Deltas.All(d => d.Data != null));
            Assert.IsTrue(deltasResponse.Deltas.All(d => d.Scope != null));
            Assert.IsTrue(deltasResponse.Deltas.All(d => d.Payer != null));
        }

        [TestMethod()]
        public async Task GetScheduleAsyncTest()
        {
            var historyClient = new HistoryClient(new HttpClientHandler());

            var scheduleResponse = await historyClient.GetScheduleAsync();

            Assert.IsNotNull(scheduleResponse.Producers);
            Assert.IsNotNull(scheduleResponse.Timestamp);
            Assert.IsTrue(scheduleResponse.Producers.All(p => p.BlockSigningKey != null));
            Assert.IsTrue(scheduleResponse.Producers.All(p => p.ProducerName != null));
        }

        [TestMethod()]
        public async Task GetTransactionAsyncTest()
        {
            var historyClient = new HistoryClient(new HttpClientHandler());

            var transactionResponse = await historyClient.GetTransactionAsync("853c70f79db05484e8511c5a10a2294326ba5255bff5996588905d9c320d442e");

            Assert.IsNotNull(transactionResponse.Actions);
            Assert.IsTrue(transactionResponse.Actions.All(a => a.Act != null));
            Assert.IsTrue(transactionResponse.Actions.All(a => a.Timestamp != null));
        }

        [TestMethod()]
        public async Task GetBlockAsyncTest()
        {
            var historyClient = new HistoryClient(new HttpClientHandler());

            uint blockNum = 10000;
            var blockResponse = await historyClient.GetBlockAsync(blockNum);

            Assert.IsNotNull(blockResponse);
            Assert.AreEqual(blockResponse.Number, blockNum);
            Assert.IsNotNull(blockResponse.Producer);
            Assert.IsNotNull(blockResponse.PreviousId);
            Assert.IsNotNull(blockResponse.Id);
        }
    }
}