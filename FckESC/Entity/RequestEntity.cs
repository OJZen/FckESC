using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FckESC.Entity
{
    abstract class RequestEntity
    {
        [JsonProperty("version")]
        public string Version { get; set; }
        [JsonProperty("username")]
        public string Username { get; set; }
        [JsonProperty("clientip")]
        public string Clientip { get; set; }
        [JsonProperty("nasip")]
        public string Nasip { get; set; }
        [JsonProperty("mac")]
        public string Mac { get; set; }
        [JsonProperty("timestamp")]
        public string Timestamp { get; set; }
        [JsonProperty("authenticator")]
        public string Authenticator { get; set; }

        public const string KEY = "Eshore!@#";
        public abstract string GetRequestUrl();
    }
}
