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
using Bayards_Android.Enums;

namespace Bayards_Android.Model
{
    public class MediaObject
    {
        public string Link { get; set; }

        public  TypeMedia  TypeMedia { get; set; }
    }
}