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

namespace Bayards_Android
{
    class Repository
    {
        public bool sendPassword(string password)
        {
            return password == "hsepassword";
        }

    }
}