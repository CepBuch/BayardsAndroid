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
using Android.Util;

namespace Bayards_Android
{
    public static class AppManager
    {
       public static void ApplyAppLanguage(Context context, string language_code)
        {
            var res = context.Resources;
            DisplayMetrics dm = res.DisplayMetrics;
            var conf = res.Configuration;
            conf.SetLocale(new Java.Util.Locale(language_code));
            res.UpdateConfiguration(conf, dm);
        }
    }
}