<div align="center">

[![builds](https://github.com/liquiidio/HyperionApiClient-Private/actions/workflows/dotnet-build.yml/badge.svg)](https://github.com/liquiidio/HyperionApiClient-Private/actions/workflows/dotnet-build.yml)
[![tests](https://github.com/liquiidio/HyperionApiClient-Private/actions/workflows/dotnet-test.yml/badge.svg)](https://github.com/liquiidio/HyperionApiClient-Private/actions/workflows/dotnet-test.yml)
       
</div>

       
# HyperionApiClient 

.NET and Unity3D-compatible (Desktop, Mobile, WebGL) ApiClient for Hyperion History APIs

## Examples

### Accounts
       var accountsClient = new AccountsClient(new HttpClient());
       var account = await accountsClient.GetAccountAsync("eosio");

### Chain
       var chainClient = new ChainClient(new HttpClient());
       var abi = await chainClient.GetAbiAsync("eosio");

### History
       var historyClient = new HistoryClient(new HttpClient());
       var actions = await historyClient.GetActionsAsync(null, null, "kingcoolcorv");

### Stats
      var statsClient = new StatsClient(new HttpClient());
      var actionUsage = await statsClient.GetActionUsageAsync("1h");

### Status
      var statusClient = new StatusClient(new HttpClient());
      var health = await statusClient.HealthAsync();

### System
      var systemClient = new SystemClient(new HttpClient());
      var voters = await systemClient.GetVotersAsync();
