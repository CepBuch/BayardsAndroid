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
        Button tryAgainButton;
        LinearLayout waitLayout;
        LinearLayout warningLayout;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.DataLoadLayout);


            _prefs = PreferenceManager.GetDefaultSharedPreferences(ApplicationContext);
            _editor = _prefs.Edit();

            tryAgainButton = FindViewById<Button>(Resource.Id.data_load_button);
            tryAgainButton.Click += LoadButton_Click;

            warningLayout = FindViewById<LinearLayout>(Resource.Id.data_warningLayout);
            waitLayout = FindViewById<LinearLayout>(Resource.Id.data_waitLayout);

            AskUserPermissionToDownload();

        }



        private void LoadButton_Click(object sender, EventArgs e)
            => DownloadData();


        //Checking internet connection, and, if it connected, asking user's permission to download data.
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

                string message = GetString(
                    isConnectedWifi ? Resource.String.data_wifi_warning : Resource.String.data_mobile_warning);

                dialog.SetMessage(message);
                //Affter clicking yes button the data will be downloaded
                dialog.SetPositiveButton("Yes", delegate { DownloadData(); });
                dialog.SetNegativeButton("No", (sender, e) => { this.Finish(); });
                dialog.SetCancelable(false);
                dialog.Show();
            }
            else
            {
                //Show error message
                string message = GetString(Resource.String.data_no_connection_waring);
                dialog.SetMessage(message);
                dialog.SetPositiveButton("Ok", delegate { this.Finish(); });
                dialog.Show();
            }
        }

        private async void DownloadData()
        {
            //Showing progress bar
            warningLayout.Visibility = ViewStates.Gone;
            waitLayout.Visibility = ViewStates.Visible;


            ApiProvider provider = new ApiProvider(_prefs.GetString("hosting_address", ""));

            string language = _prefs.GetString("languageCode", "eng");

            //Trying to download data and add it to local database
            try
            {

                var last_correct_password = _prefs.GetString("lastPassword", "");
                bool correctPassword = await provider.CheckPassword(last_correct_password);

                if (correctPassword)
                {
                    var data = await provider.GetData(new string[] { "eng", "nl" });

                    //Saving all the images onto device
                    data.ToList()
                        .ForEach(c => c.Risks.ToList()
                        .ForEach(r => r.MediaObjects.Where(m => m != null && m.TypeMedia == Enums.TypeMedia.Image).ToList()
                        .ForEach(m => SaveImage(m))));


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
                else
                {
                    var dialog = new Android.App.AlertDialog.Builder(this);
                    string message = GetString(Resource.String.password_was_changed);
                    dialog.SetMessage(message);
                    dialog.SetPositiveButton("Ok", delegate
                    {
                        _editor.PutBoolean("isAuthorised", false);
                        _editor.Apply();
                        OpenPasswordActivity();
                    });
                    dialog.Show();
                }
            }
            catch
            {
                waitLayout.Visibility = ViewStates.Gone;
                warningLayout.Visibility = ViewStates.Visible;
                tryAgainButton.Visibility = ViewStates.Visible;
            }
            finally
            {
                tryAgainButton.Enabled = true;
            }
        }

        private void OpenPasswordActivity()
        {
            Intent intent = new Intent(this, typeof(PasswordActivity));
            StartActivity(intent);
            this.Finish();
        }

        private async void SaveImage(Model.MediaObject mObj)
        {
            if (mObj != null && mObj.Bytes != null && mObj.Bytes.Length > 0)
            {
                var documentsPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
                var localFilename = mObj.Name;
                var localPath = System.IO.Path.Combine(documentsPath, localFilename);
                using (FileStream fs = new FileStream(localPath, FileMode.OpenOrCreate))
                {
                    await fs.WriteAsync(mObj.Bytes, 0, mObj.Bytes.Length);
                }
            }
        }
    }



}