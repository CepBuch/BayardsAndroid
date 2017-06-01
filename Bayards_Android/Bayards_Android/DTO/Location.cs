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
    class Location
    {
        [JsonProperty("id_l")]
        public string Id { get; set; }

        [JsonProperty("latitude")]
        public double Latitude{ get; set; }

        [JsonProperty("longitude")]
        public double Longtitude{ get; set; }

        [JsonProperty("order_v")]
        public int? Order { get; set; }
    }
}