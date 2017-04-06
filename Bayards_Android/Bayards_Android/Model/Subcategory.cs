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

namespace Bayards_Android.Model
{
    class Subcategory
    {
        public string Name { get; set; }

        public List<Risk> Risks { get; set; }
    }
}