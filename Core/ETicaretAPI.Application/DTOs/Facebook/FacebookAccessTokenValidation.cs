using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ETicaretAPI.Application.DTOs.Facebook
{
    public class FacebookAccessTokenValidation
    {
        [JsonPropertyName("data")]
        public FacebookAccessTokenValidationData Data { get; set; }
    }

    public class FacebookAccessTokenValidationData
    {

        [JsonPropertyName("is_valid")]
        public bool IsValid { get; set; }
        [JsonPropertyName("user_id")]
        public string UserId { get; set; }
    }
}
