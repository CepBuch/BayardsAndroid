using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Newtonsoft.Json;

namespace Bayards_Android.DTO
{
    class Risk
    {
        [JsonProperty("id_r")]
        public string Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }
        
        [JsonProperty("content")]
        public string Content { get; set; }

        [JsonProperty("media")]
        public  MediaObject [] MediaObjects { get; set; }  
    }
}