using System.Collections.Generic;
using Newtonsoft.Json;

namespace EosRio.HyperionApi
{
    public class Account 
    {
        [JsonProperty("name", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public string Name { get; set; }
    
        [JsonProperty("timestamp", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public string Timestamp { get; set; }
    
        [JsonProperty("trx_id", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public string TrxId { get; set; }
    }

    public class Account2
    {
        [JsonProperty("account_name")]
        public string AccountName { get; set; }

        [JsonProperty("head_block_num")]
        public int HeadBlockNum { get; set; }

        [JsonProperty("head_block_time")]
        public string HeadBlockTime { get; set; }

        [JsonProperty("privileged")]
        public bool Privileged { get; set; }

        [JsonProperty("last_code_update")]
        public string LastCodeUpdate { get; set; }

        [JsonProperty("created")]
        public string Created { get; set; }

        [JsonProperty("core_liquid_balance")]
        public string CoreLiquidBalance { get; set; }

        [JsonProperty("ram_quota")]
        public int RamQuota { get; set; }

        [JsonProperty("net_weight")]
        public int NetWeight { get; set; }

        [JsonProperty("cpu_weight")]
        public int CpuWeight { get; set; }

        [JsonProperty("net_limit")]
        public NetLimit NetLimit { get; set; }

        [JsonProperty("cpu_limit")]
        public CpuLimit CpuLimit { get; set; }

        [JsonProperty("ram_usage")]
        public int RamUsage { get; set; }

        [JsonProperty("permissions")]
        public List<Permission2> Permissions { get; set; }

        [JsonProperty("total_resources")]
        public TotalResources TotalResources { get; set; }

        [JsonProperty("self_delegated_bandwidth")]
        public object SelfDelegatedBandwidth { get; set; }

        [JsonProperty("refund_request")]
        public object RefundRequest { get; set; }

        [JsonProperty("voter_info")]
        public VoterInfo VoterInfo { get; set; }

        [JsonProperty("rex_info")]
        public object RexInfo { get; set; }

        [JsonProperty("subjective_cpu_bill_limit")]
        public SubjectiveCpuBillLimit SubjectiveCpuBillLimit { get; set; }
    }

    public class Permission2
    {
        [JsonProperty("perm_name")]
        public string PermName { get; set; }

        [JsonProperty("parent")]
        public string Parent { get; set; }

        [JsonProperty("required_auth")]
        public RequiredAuth RequiredAuth { get; set; }
    }

    public class RequiredAuth
    {
        [JsonProperty("threshold")]
        public int Threshold { get; set; }

        [JsonProperty("keys")]
        public List<string> Keys { get; set; }

        [JsonProperty("accounts")]
        public List<Account> Accounts { get; set; }

        [JsonProperty("waits")]
        public List<string> Waits { get; set; }
    }

    public class NetLimit
    {
        [JsonProperty("used")]
        public int Used { get; set; }

        [JsonProperty("available")]
        public int Available { get; set; }

        [JsonProperty("max")]
        public int Max { get; set; }
    }

    public class CpuLimit
    {
        [JsonProperty("used")]
        public int Used { get; set; }

        [JsonProperty("available")]
        public int Available { get; set; }

        [JsonProperty("max")]
        public int Max { get; set; }
    }

    public class TotalResources
    {
        [JsonProperty("owner")]
        public string Owner { get; set; }

        [JsonProperty("net_weight")]
        public string NetWeight { get; set; }

        [JsonProperty("cpu_weight")]
        public string CpuWeight { get; set; }

        [JsonProperty("ram_bytes")]
        public int RamBytes { get; set; }
    }

    public class VoterInfo
    {
        [JsonProperty("owner")]
        public string Owner { get; set; }

        [JsonProperty("proxy")]
        public string Proxy { get; set; }

        [JsonProperty("producers")]
        public List<object> Producers { get; set; }

        [JsonProperty("staked")]
        public string Staked { get; set; }

        [JsonProperty("unpaid_voteshare")]
        public string UnpaidVoteshare { get; set; }

        [JsonProperty("unpaid_voteshare_last_updated")]
        public string UnpaidVoteshareLastUpdated { get; set; }

        [JsonProperty("unpaid_voteshare_change_rate")]
        public string UnpaidVoteshareChangeRate { get; set; }

        [JsonProperty("last_claim_time")]
        public string LastClaimTime { get; set; }

        [JsonProperty("last_vote_weight")]
        public string LastVoteWeight { get; set; }

        [JsonProperty("proxied_vote_weight")]
        public string ProxiedVoteWeight { get; set; }

        [JsonProperty("is_proxy")]
        public int IsProxy { get; set; }

        [JsonProperty("flags1")]
        public int Flags1 { get; set; }

        [JsonProperty("reserved2")]
        public int Reserved2 { get; set; }

        [JsonProperty("reserved3")]
        public string Reserved3 { get; set; }
    }

    public class SubjectiveCpuBillLimit
    {
        [JsonProperty("used")]
        public int Used { get; set; }

        [JsonProperty("available")]
        public int Available { get; set; }

        [JsonProperty("max")]
        public int Max { get; set; }
    }
}