using Android.App;
using Android.Widget;
using Android.OS;
using Android.Util;
using Android.Content;
using Android.Preferences;
using Android.Support.V7.App;
using Android.Views;

namespace Bayards_Android
{
    [Activity(MainLauncher = true, Theme = "@style/Theme.AppCompat.Light.NoActionBar")]
    public class MainActivity : ActionBarActivity
    {
        ISharedPreferences _prefs;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);


            //Getting info about user's authorization process from shared preferences.
            _prefs = PreferenceManager.GetDefaultSharedPreferences(ApplicationContext);
            var isLanguageChosen = _prefs.GetBoolean("isLanguageChosen", false);
            var isAuthorized = _prefs.GetBoolean("isAuthorized", false);
            var isAcceptedAgreement = _prefs.GetBoolean("isAcceptedAgreement", false);

            //If language was chosen, setting the appropriate one.
            if (isLanguageChosen)
            {
                string language_code = _prefs.GetString("languageCode", "en");
                applyAppLanguage(language_code);
            }

            //Showing the corresponding authorizatin page.
            //If all checks have been completed, just continue. 
            if (!isLanguageChosen || !isAuthorized || !isAcceptedAgreement)
            {
                Intent intent;
                if (!isLanguageChosen)
                    intent = new Intent(this, typeof(LanguageActivity));
                else if (!isAuthorized)
                    intent = new Intent(this, typeof(PasswordActivity));
                else
                    intent = new Intent(this, typeof(AgreementActivity));

                StartActivity(intent);
                this.Finish();
            }

            //Setting up the main page
            SetContentView(Resource.Layout.MainLayout);
            //Accepting custom toolbar 
            Android.Support.V7.Widget.Toolbar toolbar =
                FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar_main);
            SetSupportActionBar(toolbar);
            //Disabling default title (as custom title with another style is shown (toolbar_main.xml)
            SupportActionBar.SetDisplayShowTitleEnabled(false);



        }

        protected void applyAppLanguage(string language_code)
        {
            var res = this.Resources;
            DisplayMetrics dm = res.DisplayMetrics;
            var conf = res.Configuration;
            conf.SetLocale(new Java.Util.Locale(language_code));
            res.UpdateConfiguration(conf, dm);
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.menu_main, menu);
            return base.OnPrepareOptionsMenu(menu);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Resource.Id.menu_logout:
                    {
                        //Logout process
                        ISharedPreferencesEditor editor = _prefs.Edit();
                        editor.PutBoolean("isLanguageChosen", false);
                        editor.PutBoolean("isAuthorized", false);
                        editor.PutBoolean("isAcceptedAgreement", false);
                        editor.Apply();
                        this.Recreate();
                        return true;
                    }
                default:
                    return base.OnOptionsItemSelected(item);
            }
        }







    }
}

