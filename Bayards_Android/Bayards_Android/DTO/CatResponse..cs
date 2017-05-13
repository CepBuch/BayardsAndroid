using Newtonsoft.Json;

namespace Bayards_Android.DTO
{
    class CatResponse
    {
        [JsonProperty("section")]
        public Category Category { get; set; }
    }
}