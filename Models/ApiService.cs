using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace marker_dotnet.Models
{
    public class ApiResponse
    {
        [JsonProperty("success")]
        public bool Success { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("invoice")]
        public string Invoice { get; set; }
    }

    public class PrintRequest
    {
        [JsonProperty("jumlah_marker_diminta")]
        public int JumlahMarkerDiminta { get; set; }

        [JsonProperty("device")]
        public string Device { get; set; }
    }

    public class StatusUpdateRequest
    {
        [JsonProperty("invoice")]
        public string Invoice { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("error_message")]
        public string ErrorMessage { get; set; }
    }
}
