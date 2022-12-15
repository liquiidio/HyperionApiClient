

<div align="center">
 <img src="https://avatars.githubusercontent.com/u/82725791?s=200&v=4" align="center"
     alt="Liquiid logo" width="280" height="300">
</div>
# HyperionApiClient 

<div align="center">

[![builds](https://github.com/liquiidio/HyperionApiClient-Private/actions/workflows/dotnet-build.yml/badge.svg)](https://github.com/liquiidio/HyperionApiClient-Private/actions/workflows/dotnet-build.yml)
[![tests](https://github.com/liquiidio/HyperionApiClient-Private/actions/workflows/dotnet-test.yml/badge.svg)](https://github.com/liquiidio/HyperionApiClient-Private/actions/workflows/dotnet-test.yml)
       
</div>

.NET and Unity3D-compatible (Desktop, Mobile, WebGL) Client-Library for Hyperion History APIs

See [Hyperion History by EosRio](https://eosrio.io/hyperion/)

### INSTALLATION
---
A step by step series of examples that tell you how to get a package. There is the Unity package,Unity Package Manager GitHub and Nuget Package.
 

1. Say what the steps will be for each package

    Give the example

2. A step by step explanation on how to clone and import the repository

    give the instructions

---
## Usage
.NET and Unity3D-compatible (Desktop, Mobile, WebGL) ApiClient for the different  APIs. 
Endpoints have its own set of parameters that you may build up and pass in to the relevant function.

### Examples

#### Accounts
```csharp
       var accountsClient = new AccountsClient(new HttpClient());
       var account = await accountsClient.GetAccountAsync("eosio");
```

#### Chain
```csharp
       var chainClient = new ChainClient(new HttpClient());
       var abi = await chainClient.GetAbiAsync("eosio");
```

#### History
```csharp
       var historyClient = new HistoryClient(new HttpClient());
       var actions = await historyClient.GetActionsAsync(null, null, "kingcoolcorv");
```

#### Stats
```csharp
       var statsClient = new StatsClient(new HttpClient());
       var actionUsage = await statsClient.GetActionUsageAsync("1h");
```

#### Status
```csharp
       var statusClient = new StatusClient(new HttpClient());
       var health = await statusClient.HealthAsync();
```

#### System
```csharp
       var systemClient = new SystemClient(new HttpClient());
       var voters = await systemClient.GetVotersAsync();
```
