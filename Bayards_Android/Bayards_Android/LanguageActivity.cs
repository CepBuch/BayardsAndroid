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
    [Activity(Theme = "@android:style/Theme.DeviceDefault.Light.NoActionBar")]
    public class LanguageActivity : Activity
    {

        ISharedPreferences _prefs;
        ISharedPreferencesEditor _editor;
        LinearLayout engLayout;
        LinearLayout nlLayout;
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.LanguageActivity);

            _prefs = PreferenceManager.GetDefaultSharedPreferences(ApplicationContext);
            _editor = _prefs.Edit();

            FindViews();

            engLayout.Click += (sender, e) => ChooseLanguage("eng");
            nlLayout.Click += (sender, e) => ChooseLanguage("nl");
        }

        protected void FindViews()
        {
            engLayout = FindViewById<LinearLayout>(Resource.Id.EngLayout);
            nlLayout = FindViewById<LinearLayout>(Resource.Id.NlLayout);
        }

        protected void ChooseLanguage(string language_code)
        {
            //Applying chosen language
            AppManager.ApplyAppLanguage(this, language_code);

            Intent intent = new Intent(this, typeof(MainActivity));
            StartActivity(intent);

            //Remember that language is chosen.
            _editor.PutBoolean("isLanguageChosen", true);
            _editor.PutString("languageCode", language_code);
            _editor.Apply();
            this.Finish();
        }





    }
}