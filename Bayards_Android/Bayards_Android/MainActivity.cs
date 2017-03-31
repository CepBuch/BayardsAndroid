﻿using Android.App;
using Android.Widget;
using Android.OS;
using Android.Util;
using Android.Content;
using Android.Preferences;

namespace Bayards_Android
{
    [Activity(Label = "Bayards_Android", MainLauncher = true,
        Theme = "@android:style/Theme.DeviceDefault.Light")]
    public class MainActivity : Activity
    {
        ISharedPreferences _prefs;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.MainLayout);
            _prefs = PreferenceManager.GetDefaultSharedPreferences(ApplicationContext);

            //Checking whether the user is authorized. Otherwise open authorization page
            var isLanguageChosen = _prefs.GetBoolean("isLanguageChosen", false);
            var isAuthorized = _prefs.GetBoolean("isAuthorized", false);
            var isAcceptedAgreement = _prefs.GetBoolean("isAcceptedAgreement", false);

            string language_code = "en";
            if (isLanguageChosen)
            {
                language_code = _prefs.GetString("languageCode", "en");
                applyAppLanguage(language_code);
            }

            //Temporary
            TextView languageTxt = FindViewById<TextView>(Resource.Id.languageTxt);
            TextView authTxt = FindViewById<TextView>(Resource.Id.authTxt);
            TextView agreementTxt = FindViewById<TextView>(Resource.Id.agreementTxt);
            Button logoutBtn = FindViewById<Button>(Resource.Id.logout_button);
            ISharedPreferencesEditor editor = _prefs.Edit();
            languageTxt.Text  += isLanguageChosen.ToString();
            authTxt.Text += isAuthorized.ToString();
            agreementTxt.Text += isAcceptedAgreement.ToString();
            logoutBtn.Click += delegate
             {
                 editor.PutBoolean("isLanguageChosen", false);
                 editor.PutBoolean("isAuthorized", false);
                 editor.PutBoolean("isAcceptedAgreement", false);
                 editor.Apply();
                 this.Recreate();
             };
            //End of temporary

            if (!isLanguageChosen || !isAuthorized || !isAcceptedAgreement)
            {
                Intent intent;
                if (!isLanguageChosen)
                    intent = new Intent(this, typeof(LanguageActivity));
                else if(!isAuthorized)
                    intent = new Intent(this, typeof(PasswordActivity));
                else
                    intent = new Intent(this, typeof(AgreementActivity));

                StartActivity(intent);
                this.Finish();
            }
            //else
            //{
            //    SetContentView(Resource.Layout.MainLayout);
            //}
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

