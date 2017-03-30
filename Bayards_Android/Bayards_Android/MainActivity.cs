using Android.App;
using Android.Widget;
using Android.OS;
using Android.Util;
using Android.Content;

namespace Bayards_Android
{
    [Activity(Label = "Bayards_Android", MainLauncher = true, Icon = "@drawable/icon",
        Theme = "@android:style/Theme.DeviceDefault.Light.NoActionBar")]
    public class MainActivity : Activity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.LanguageSelectionScreen);

            LinearLayout engLayout = FindViewById<LinearLayout>(Resource.Id.EngLayout);
            LinearLayout nlLayout = FindViewById<LinearLayout>(Resource.Id.NlLayout);

            engLayout.Click += (sender, e) => chooseLanguage("en");
            nlLayout.Click += (sender, e) => chooseLanguage("nl");
        }

        protected void chooseLanguage(string language_code)
        {
            changeAppLanguage(language_code);
            var intent = new Intent(this, typeof(PasswordActivity));
            StartActivity(intent);
        }


        protected void changeAppLanguage(string language_code)
        {
            var res = this.Resources;
            DisplayMetrics dm = res.DisplayMetrics;
            var conf = res.Configuration;
            conf.SetLocale(new Java.Util.Locale(language_code));
            res.UpdateConfiguration(conf, dm);
        }
    }
}

