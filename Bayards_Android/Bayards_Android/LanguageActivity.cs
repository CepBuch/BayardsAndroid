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
using Android.Preferences;

namespace Bayards_Android
{
    [Activity(Label = "Login page", Theme = "@android:style/Theme.DeviceDefault.Light.NoActionBar")]
    public class LanguageActivity : Activity
    {

        ISharedPreferences _prefs;
        ISharedPreferencesEditor _editor;
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.LanguageActivity);


            _prefs = PreferenceManager.GetDefaultSharedPreferences(ApplicationContext);
            _editor = _prefs.Edit();


            LinearLayout engLayout = FindViewById<LinearLayout>(Resource.Id.EngLayout);
            LinearLayout nlLayout = FindViewById<LinearLayout>(Resource.Id.NlLayout);

            engLayout.Click += (sender, e) => chooseLanguage("eng");
            nlLayout.Click += (sender, e) => chooseLanguage("nl");
        }

        protected void chooseLanguage(string language_code)
        {
            applyAppLanguage(language_code);
            Intent intent = new Intent(this, typeof(MainActivity));             

            StartActivity(intent);

            //Remember that language is chosen.
            _editor.PutBoolean("isLanguageChosen", true);
            _editor.PutString("languageCode", language_code);
            _editor.Apply();
            this.Finish();
        }

        protected void applyAppLanguage(string language_code)
        {
            var res = this.Resources;
            DisplayMetrics dm = res.DisplayMetrics;
            var conf = res.Configuration;
            conf.SetLocale(new Java.Util.Locale(language_code));
            res.UpdateConfiguration(conf, dm);
        }



    }
}