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
    class MediaObject
    {
        [JsonProperty("link_m")]
        public string Link { get; set; }

        [JsonProperty("type_media")]
        public string TypeMedia { get; set; }
    }
}