using Newtonsoft.Json;

namespace EthioNegarit.Utilities
{
    public  class IpInfo
    {
        [JsonProperty("ip")]
        public string Ip { get; set; }

        [JsonProperty("hostname")]
        public string Hostname { get; set; }

        [JsonProperty("city")]
        public string City { get; set; }

        [JsonProperty("region")]
        public string Region { get; set; }

        [JsonProperty("country")]
        public string Country { get; set; }
        [JsonProperty("country_name")]
        public string Country_Name { get; set; }

        [JsonProperty("NativeName")]
        public string NativeName { get; set; }
        
        [JsonProperty("loc")]
        public string Loc { get; set; }
        public string lat { get; set; }
        public string lon { get; set; }
        [JsonProperty("org")]
        public string Org { get; set; }

        [JsonProperty("postal")]
        public string Postal { get; set; }
        [JsonProperty("geoid")]
        public int GeoId { get; set; }
        public string timezone { get; set; }
    }
}
