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
using Android.Preferences;
using Android.Support.V7.App;
using SupportToolbar = Android.Support.V7.Widget.Toolbar;
using Android.Text.Method;
using Android.Net;

namespace Bayards_Android
{
    [Activity(Theme = "@style/AppTheme")]
    public class SettingsActivity : AppCompatActivity
    {
        ISharedPreferences _prefs;
        ISharedPreferencesEditor _editor;
        SupportToolbar toolbar;
        TextView linkTextView;
        TextView lastUpdateTextView;
        Button languageSettingsButton;
        Button checkUpdatesButton;
        ProgressBar checkProgressBar;
        string language;
        DateTime lastUpdateDate = default(DateTime);    

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.SettingsActivity);


            _prefs = PreferenceManager.GetDefaultSharedPreferences(ApplicationContext);
            _editor = _prefs.Edit();
            language = _prefs.GetString("languageCode", "eng");
           
            FindViews();
            CustomizeToolbar();
            string date = _prefs.GetString("lastUpdateDate", GetString(Resource.String.date_unknown));
            DateTime.TryParse(date, out lastUpdateDate);
            lastUpdateTextView.Text = $"{GetString(Resource.String.last_update)}: {date}";
            languageSettingsButton.Click += (e, s) => SelectLanguage();
            checkUpdatesButton.Click += (e, s) => CheckUpdates();
        }



        public void FindViews()
        {
            linkTextView = FindViewById<TextView>(Resource.Id.websiteTextView);
            linkTextView.MovementMethod = LinkMovementMethod.Instance;
            lastUpdateTextView = FindViewById<TextView>(Resource.Id.lastUpdateTextView);
            languageSettingsButton = FindViewById<Button>(Resource.Id.languageButton);
            checkUpdatesButton = FindViewById<Button>(Resource.Id.updatesButton);
            checkProgressBar = FindViewById<ProgressBar>(Resource.Id.check_progressBar);

        }


        public void CustomizeToolbar()
        {
            //Accepting custom toolbar 
            toolbar = FindViewById<SupportToolbar>(Resource.Id.toolbar_settings);
            TextView toolbarTitle = FindViewById<TextView>(Resource.Id.toolbar_title);
            SetSupportActionBar(toolbar);
            //Disabling default title and showing title from resources
            SupportActionBar.SetDisplayShowTitleEnabled(false);
            toolbarTitle.Text = Resources.GetString(Resource.String.nav_settings);
            //Enabling BackButton
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            SupportActionBar.SetDisplayShowHomeEnabled(true);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Android.Resource.Id.Home:
                    {
                        Intent returnIntent = new Intent();
                        SetResult(Result.Canceled, returnIntent);
                        Finish();
                        return true;
                    }
                default:
                    return base.OnOptionsItemSelected(item);
            }
        }

        public void SelectLanguage()
        {
            string[] languages = Resources.GetStringArray(Resource.Array.languagesArray);
            var builder = new Android.App.AlertDialog.Builder(this);
            builder.SetTitle(Resource.String.lan_title);
            builder.SetSingleChoiceItems(languages, language == "eng" ? 0 : 1, (o, e) =>
               {
                   if (e.Which == 0 && language != "eng")
                   {
                       ApplyLanguage("eng");
                   }
                   else if (e.Which == 1 && language != "nl")
                   {
                       ApplyLanguage("nl");

                   }
                   ((Dialog)o).Dismiss();
               });
            builder.Show();
        }

        private void ApplyLanguage(string language_code)
        {
            //Change language
            AppManager.ApplyAppLanguage(this, language_code);
            Toast.MakeText(this, GetString(Resource.String.lan_aplied), ToastLength.Long).Show();
            _editor.PutString("languageCode", language_code);
            _editor.Apply();

            //Return result(language was chosen => main activity will be recreated by OnActivityResult)
            Intent returnIntent = new Intent();
            SetResult(Result.Ok, returnIntent);
            Finish();
        }

        private async void CheckUpdates()
        {
            var dialog = new Android.App.AlertDialog.Builder(this);
            if (CheckConnection())
            {
                checkUpdatesButton.Visibility = ViewStates.Invisible;
                checkProgressBar.Visibility = ViewStates.Visible;
                var host = _prefs.GetString("hosting_address", "");
                var token = _prefs.GetString("token", "");

                ApiProvider provider = new ApiProvider(host, token);

                try
                {
                    bool outdated = await provider.CheckUpdates(lastUpdateDate);
                    checkUpdatesButton.Visibility = ViewStates.Visible;
                    checkProgressBar.Visibility = ViewStates.Invisible;
                    if (outdated)
                    {
                        string message = GetString(Resource.String.update_message);
                        dialog.SetMessage(message);

                        //Affter clicking yes button the data will be downloaded
                        dialog.SetPositiveButton(GetString(Resource.String.yes), (sender, e) => { OpenDataLoadActivity(); });
                        dialog.SetNegativeButton(GetString(Resource.String.no), (sender, e) => { });
                        dialog.SetCancelable(false);
                        dialog.Show();
                    }
                    else
                    {
                        string message = GetString(Resource.String.update_latest);
                        dialog.SetMessage(message);
                        dialog.SetPositiveButton("Ok", delegate { });
                        dialog.Show();
                    }
                }
                catch { }
            }
            else
            {
                string message = GetString(Resource.String.update_no_connection);
                string title = GetString(Resource.String.error);
                dialog.SetTitle(title);
                dialog.SetMessage(message);
                dialog.SetPositiveButton("Ok", delegate { });
                dialog.Show();
            }
        }

        private bool CheckConnection()
        {
            ConnectivityManager connectivityManager = (ConnectivityManager)GetSystemService(ConnectivityService);
            NetworkInfo info = connectivityManager.ActiveNetworkInfo;

            return (info != null && info.IsConnected);
        }
        private void OpenDataLoadActivity()
        {
            Intent intent = new Intent(this, typeof(DataLoadActivity));
            intent.PutExtra("enableBackButton", true);
            StartActivity(intent);
        }
    }



}