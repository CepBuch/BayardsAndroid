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
    class Response
    {
        [JsonProperty("sections")]
        public Category [] Categories { get; set; }

        [JsonProperty("locations")]
        public Location [] Locations { get; set; }

        [JsonProperty("date")]
        public DateTime UpdateDate { get; set; }
    }
}