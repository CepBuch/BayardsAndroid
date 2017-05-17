using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.App;
using Android.Content;
using Android.OS;
using System.IO;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Preferences;
using System.Data;
using Mono.Data.Sqlite;
using Android.Net;
using System.Threading.Tasks;

namespace Bayards_Android
{
    [Activity(Theme = "@style/Theme.AppCompat.Light.NoActionBar")]

    public class DataLoadActivity : Activity
    {
        ISharedPreferences _prefs;
        ISharedPreferencesEditor _editor;
        Button loadButton;
        LinearLayout waitLayout;
        LinearLayout warningLayout;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.DataLoadLayout);


            _prefs = PreferenceManager.GetDefaultSharedPreferences(ApplicationContext);
            _editor = _prefs.Edit();

            loadButton = FindViewById<Button>(Resource.Id.data_load_button);
            loadButton.Click += LoadButton_Click;

            warningLayout = FindViewById<LinearLayout>(Resource.Id.data_warningLayout);
            waitLayout = FindViewById<LinearLayout>(Resource.Id.data_waitLayout);

        }


        //При нажатии кнопки проверяем соединение с интернетом, и , если успешно спрашиваем пользователя о согласии
        private void LoadButton_Click(object sender, EventArgs e)
            => AskUserPermissionToDownload();


        private void AskUserPermissionToDownload()
        {
            ConnectivityManager connectivityManager = (ConnectivityManager)GetSystemService(ConnectivityService);
            NetworkInfo info = connectivityManager.ActiveNetworkInfo;
            var dialog = new Android.App.AlertDialog.Builder(this);

            //If connected to the internet
            if (info != null && info.IsConnected)
            {
                // Check if connection type is wifi
                bool isConnectedWifi = info.Type == ConnectivityType.Wifi;

                //Show appropriate message
                string message = GetString(
                    isConnectedWifi ? Resource.String.data_wifi_warning : Resource.String.data_mobile_warning);

                dialog.SetMessage(message);
                dialog.SetPositiveButton("Yes", delegate { DownloadData(); });
                dialog.SetNegativeButton("No", (sender, e) => { });
                dialog.Show();
            }
            else
            {
                //Show error message
                string message = GetString(Resource.String.data_no_connection_waring);
                dialog.SetMessage(message);
                dialog.SetPositiveButton("Ok", delegate { });
                dialog.Show();
            }
        }

        private async void DownloadData()
        {
            //Showing progress bar
            warningLayout.Visibility = ViewStates.Gone;
            waitLayout.Visibility = ViewStates.Visible;
            loadButton.Enabled = false;


            ApiProvider provider = new ApiProvider();
            string language = _prefs.GetString("languageCode", "eng");

            //Trying to download data and add it to local database
            try
            {
                var data = await provider.GetData(new string [] {"eng", "nl"});

                bool createStatus = Database.Manager.CreateDatabase();

                if (createStatus)
                {
                    bool saveStatus = Database.Manager.SaveData(data);

                    if (saveStatus)
                    {
                        _editor.PutBoolean("isDataLoaded", true);
                        _editor.Apply();

                        var intent = new Intent(this, typeof(MainActivity));
                        StartActivity(intent);
                        this.Finish();
                    }
                    else throw new SqliteException("Problems with saving data to Database");
                }
                else throw new SqliteException("Database was not created");

               
            }
            catch
            {
                waitLayout.Visibility = ViewStates.Gone;
                warningLayout.Visibility = ViewStates.Visible;
            }
            finally
            {
                loadButton.Enabled = true;
            }
        }




    }



}