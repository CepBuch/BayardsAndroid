using Android.App;
using Android.Widget;
using Android.OS;
using Android.Util;
using Android.Content;
using Android.Preferences;
using Android.Support.V7.App;
using Android.Views;
using Android.Support.V7.Widget;
using SupportToolbar = Android.Support.V7.Widget.Toolbar;
using Bayards_Android.CategoryViewModel;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using Bayards_Android.Model;

using System;
using Android.Support.V4.Widget;
using Bayards_Android.Fragments;
using Android.Support.Design.Widget;
using Android.Runtime;
using Android.Net;

namespace Bayards_Android
{
    [Activity(MainLauncher = true, Theme = "@style/AppTheme")]
    public class MainActivity : AppCompatActivity
    {
        ISharedPreferences prefs;
        ISharedPreferencesEditor editor;
        private SupportToolbar toolbar;
        private ActionBarDrawerToggle drawerToggle;
        private DrawerLayout drawerLayout;
        private NavigationView navigationView;
        string language;
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            prefs = PreferenceManager.GetDefaultSharedPreferences(ApplicationContext);
            editor = prefs.Edit();

            //--------------------------------------------------------------------------------
            //-----------------------PUTTING THE SERVER ADDRESS-------------------------------
            editor.PutString("hosting_address", "http://vhost29450.cpsite.ru");
            editor.Apply();
            //--------------------------------------------------------------------------------
            //--------------------------------------------------------------------------------

            //Check user's authorization stage
            bool passedAllChecks = CheckStepsOfAuthorization();
            bool hasRecords = CountCategories();

            if (passedAllChecks && hasRecords)
            {
                SetContentView(Resource.Layout.MainActivity);
                CustomizeToolbarAndNavView();
                ShowMainContent();
                CheckUpdates();
            }
        }

        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            //This recreates activity to apply changes made in settingsactivity
            //For example if language was changed or data was updated.
            if (requestCode == 1 && resultCode == Result.Ok)
            {
                this.Recreate();
            }
        }
        public void CustomizeToolbarAndNavView()
        {
            //Accepting toolbar and drawerlayout
            toolbar = FindViewById<SupportToolbar>(Resource.Id.toolbar_main);
            drawerLayout = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);
            SetSupportActionBar(toolbar);

            drawerToggle = new ActionBarDrawerToggle(this, drawerLayout,
                Resource.String.openDrawer, Resource.String.closeDrawer);

            drawerLayout.AddDrawerListener(drawerToggle);
            SupportActionBar.SetHomeButtonEnabled(true);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            SupportActionBar.SetDisplayShowTitleEnabled(false);
            drawerToggle.SyncState();
            navigationView = FindViewById<NavigationView>(Resource.Id.nav_view);

            navigationView.NavigationItemSelected += NavigationView_NavigationItemSelected;
        }



        private bool CountCategories()
        {
            var numCategories = Database.Manager.CountCategories(language);
            if (numCategories.HasValue && numCategories > 0)
            {
                return true;
            }
            else
            {
                var dialog = new Android.App.AlertDialog.Builder(this);
                string message = GetString(Resource.String.data_reading_error);
                dialog.SetMessage(message);
                dialog.SetPositiveButton("Ok", delegate
                {
                    editor.PutBoolean("isDataLoaded", false);
                    editor.Apply();
                    CheckStepsOfAuthorization();
                });
                dialog.Show();
                return false;
            }
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            navigationView.InflateMenu(Resource.Menu.nav_menu);
            MenuInflater.Inflate(Resource.Menu.menu_main, menu);
            return base.OnPrepareOptionsMenu(menu);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            drawerToggle.OnOptionsItemSelected(item);
            switch (item.ItemId)
            {
                default:
                    return base.OnOptionsItemSelected(item);
            }
        }

        private void NavigationView_NavigationItemSelected(object sender, NavigationView.NavigationItemSelectedEventArgs e)
        {
            {
                switch (e.MenuItem.ItemId)
                {
                    case Resource.Id.nav_exit:
                        {
                            var dialog = new Android.App.AlertDialog.Builder(this);
                            dialog.SetMessage(GetString(Resource.String.logout_message));

                            dialog.SetPositiveButton("Yes", delegate { LogOut(); });
                            dialog.SetNegativeButton("Cancel", delegate { });
                            dialog.Show();
                            break;
                        }
                    case Resource.Id.nav_locations:
                        {
                            Toast.MakeText(this, "Locations clicked", ToastLength.Long).Show();
                            break;
                        }
                    case Resource.Id.nav_home:
                        {
                            ShowMainContent();
                            break;
                        }
                    case Resource.Id.nav_settings:
                        {
                            ShowSettings();
                            break;
                        }
                }
                e.MenuItem.SetChecked(true);
                drawerLayout.CloseDrawers();
            };
        }

        private void ShowMainContent()
        {
            var trans = SupportFragmentManager.BeginTransaction();
            var categoriesContainerFragment = CategoriesContainerFragment.newInstance(string.Empty);
            trans.Replace(Resource.Id.mainFragmentContainer, categoriesContainerFragment);
            trans.Commit();
        }

        public void ShowSettings()
        {
            var intent = new Intent(this, typeof(SettingsActivity));
            StartActivityForResult(intent, 1);
        }


        public bool CheckStepsOfAuthorization()
        {
            //Getting info about user's authorization process from shared preferences.
            var isLanguageChosen = prefs.GetBoolean("isLanguageChosen", false);
            var isAuthorized = prefs.GetBoolean("isAuthorized", false);
            var isAcceptedAgreement = prefs.GetBoolean("isAcceptedAgreement", false);
            var isDataLoaded = prefs.GetBoolean("isDataLoaded", false);

            //If language was chosen, setting the appropriate one.
            if (isLanguageChosen)
            {
                language = prefs.GetString("languageCode", "eng");
                AppManager.ApplyAppLanguage(this, language);
            }

            //Showing the corresponding authorizatin page.
            //If all checks have been completed, just continue. 
            if (!isLanguageChosen || !isAuthorized || !isAcceptedAgreement || !isDataLoaded)
            {
                Intent intent;
                if (!isLanguageChosen)
                    intent = new Intent(this, typeof(LanguageActivity));
                else if (!isAuthorized)
                    intent = new Intent(this, typeof(PasswordActivity));
                else if (!isAcceptedAgreement)
                    intent = new Intent(this, typeof(AgreementActivity));
                else
                    intent = new Intent(this, typeof(DataLoadActivity));

                StartActivity(intent);
                this.Finish();
                return false;
            }

            return true;
        }

        /// <summary>
        /// This method checks for update asynchroneous when application starts
        /// and device is connected to the internet
        /// </summary>
        public async void CheckUpdates()
        {
            ConnectivityManager connectivityManager = (ConnectivityManager)GetSystemService(ConnectivityService);
            NetworkInfo info = connectivityManager.ActiveNetworkInfo;
            DateTime lastUpdateDate = default(DateTime);
            string date = prefs.GetString("lastUpdateDate", GetString(Resource.String.date_unknown));
            
            if (info != null && info.IsConnected && DateTime.TryParse(date, out lastUpdateDate))
            {
                var host = prefs.GetString("hosting_address", "");
                var token = prefs.GetString("token", "");
                ApiProvider provider = new ApiProvider(host, token);
                try
                {
                    bool outdated = await provider.CheckUpdates(lastUpdateDate);
                    if (outdated)
                    {
                        var dialog = new Android.App.AlertDialog.Builder(this);
                        string message = GetString(Resource.String.update_message);
                        dialog.SetMessage(message);

                        //Affter clicking yes button the data will be downloaded
                        dialog.SetPositiveButton(GetString(Resource.String.yes), (sender, e) => { OpenDataLoadActivity(); });
                        dialog.SetNegativeButton(GetString(Resource.String.no), (sender, e) => { });
                        dialog.SetCancelable(false);
                        dialog.Show();
                    }
                }
                catch { }
            }
        }
        private void OpenDataLoadActivity()
        {
            Intent intent = new Intent(this, typeof(DataLoadActivity));
            intent.PutExtra("enableBackButton", true);
            StartActivity(intent);
        }


        public void LogOut()
        {
            //Logout process: set all steps of authorization as false
            editor.PutBoolean("isLanguageChosen", false);
            editor.PutBoolean("isAuthorized", false);
            editor.PutBoolean("isAcceptedAgreement", false);
            editor.PutBoolean("isDataLoaded", false);
            editor.Apply();

            //and reload this (main) activity
            this.Recreate();
        }
    }
}

