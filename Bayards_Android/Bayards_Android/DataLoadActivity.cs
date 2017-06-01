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
using Bayards_Android.Model;
using SupportToolbar = Android.Support.V7.Widget.Toolbar;
using Android.Support.V7.App;

namespace Bayards_Android
{
    [Activity(Theme = "@style/AppTheme")]


    public class DataLoadActivity : AppCompatActivity
    {
        ISharedPreferences _prefs;
        ISharedPreferencesEditor _editor;
        Button tryAgainButton;
        LinearLayout waitLayout;
        LinearLayout warningLayout;
        SupportToolbar toolbar;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.DataLoadActivity);


            _prefs = PreferenceManager.GetDefaultSharedPreferences(ApplicationContext);
            _editor = _prefs.Edit();

            FindViews();
            CustomizeToolbar();
            tryAgainButton.Click += delegate { DownloadData(); };
            AskUserPermissionToDownload();
        }

        private void FindViews()
        {
            tryAgainButton = FindViewById<Button>(Resource.Id.data_load_button);
            warningLayout = FindViewById<LinearLayout>(Resource.Id.data_warningLayout);
            waitLayout = FindViewById<LinearLayout>(Resource.Id.data_waitLayout);
        }
        
        private void CustomizeToolbar()
        {
            var enableBackButton = Intent.GetBooleanExtra("enableBackButton", false);
            if (enableBackButton)
            {
                //Enabling custom toolbar
                toolbar = FindViewById<SupportToolbar>(Resource.Id.toolbar_dataload);
                SetSupportActionBar(toolbar);

                //Disabling default title;
                SupportActionBar.SetDisplayShowTitleEnabled(false);
                //Enabling BackButton
                SupportActionBar.SetDisplayHomeAsUpEnabled(true);
                SupportActionBar.SetDisplayShowHomeEnabled(true);
            }
        }
        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Android.Resource.Id.Home:
                    {
                        Finish();
                        return true;
                    }
                default:
                    return base.OnOptionsItemSelected(item);
            }
        }




        //Checking internet connection, and, if it connected, asking user's permission to download data.
        private void AskUserPermissionToDownload()
        {
            ConnectivityManager connectivityManager = (ConnectivityManager)GetSystemService(ConnectivityService);
            NetworkInfo info = connectivityManager.ActiveNetworkInfo;
            var dialog = new Android.App.AlertDialog.Builder(this);

            //If connected to the internet
            if (info != null && info.IsConnected)
            {
                string message = GetString(Resource.String.data_download_warning);
                string title = GetString(Resource.String.warning);
                dialog.SetTitle(title);
                dialog.SetMessage(message);

                //Affter clicking yes button the data will be downloaded
                dialog.SetPositiveButton(GetString(Resource.String.yes), delegate { DownloadData(); });
                dialog.SetNegativeButton(GetString(Resource.String.no), (sender, e) => { this.Finish(); });
                dialog.SetCancelable(false);
                dialog.Show();
            }
            else
            {
                //Show error message
                string message = GetString(Resource.String.data_no_connection_waring);
                string title = GetString(Resource.String.error);
                dialog.SetTitle(title);
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
            tryAgainButton.Enabled = false;

            var host = _prefs.GetString("hosting_address", "");
            var token = _prefs.GetString("token", "");
            string language = _prefs.GetString("languageCode", "eng");

            ApiProvider provider = new ApiProvider(host, token);

            //Trying to download data and add it to local database
            try
            {
                bool correctPassword = await provider.CheckPassword(token);

                if (correctPassword)
                {
                    var data = await provider.GetData(new string[] { "eng", "nl" });
                    var categories = data.Item1;
                    var locations = data.Item2;
                    var lastUpdateDate = data.Item3;

                    //Saving all the images onto device
                    SaveAllImages(categories);

                    bool createStatus = Database.Manager.CreateDatabase();

                    if (createStatus)
                    {
                        bool saveStatus = Database.Manager.SaveData(categories, locations);

                        if (saveStatus)
                        {
                            _editor.PutBoolean("isDataLoaded", true);
                            _editor.PutString("lastUpdateDate", lastUpdateDate.ToString());
                            _editor.Apply();

                            var intent = new Intent(this, typeof(MainActivity));
                            StartActivity(intent);
                            this.Finish();
                        }
                        else throw new SqliteException("Problems with saving data to Database");
                    }
                    else throw new SqliteException("Database was not created");
                }
                else throw new UnauthorizedAccessException();
                
            }
            catch (UnauthorizedAccessException)
            {
                waitLayout.Visibility = ViewStates.Gone;
                warningLayout.Visibility = ViewStates.Visible;
                tryAgainButton.Visibility = ViewStates.Visible;
                tryAgainButton.Enabled = true;

                var dialog = new Android.App.AlertDialog.Builder(this);
                string message = GetString(Resource.String.password_was_changed);
                dialog.SetMessage(message);
                dialog.SetPositiveButton("Ok", delegate
                {
                    OpenPasswordActivity();
                });
                dialog.SetNegativeButton(GetString(Resource.String.cancel), delegate { } );

                dialog.Show();
            }
            catch (Exception ex)
            {
                waitLayout.Visibility = ViewStates.Gone;
                warningLayout.Visibility = ViewStates.Visible;
                tryAgainButton.Visibility = ViewStates.Visible;
                tryAgainButton.Enabled = true;
            }
        }

        private void OpenPasswordActivity()
        {
            Intent intent = new Intent(this, typeof(PasswordActivity));
            StartActivity(intent);
            this.Finish();
        }

        private async void SaveAllImages(Category[] categories)
        {
            try
            {
                List<MediaObject> mediaWithImagesPaths = new List<MediaObject>();

                if (categories != null)
                {
                    //Selecting all mediaobjects that have images
                    foreach (var category in categories.Where(c => c != null))
                    {
                        if (category.Risks != null)
                            foreach (var risk in category.Risks.Where(r => r != null))
                                if (risk.MediaObjects != null)
                                    foreach (var media in risk.MediaObjects.Where(o => !string.IsNullOrWhiteSpace(o.Name) && o.Bytes != null && o.TypeMedia == Enums.TypeMedia.Image))
                                        mediaWithImagesPaths.Add(media);

                        if (category.Subcategories != null)
                            foreach (var subcat in category.Subcategories.Where(sc => sc != null))
                                if (subcat.Risks != null)
                                    foreach (var risk in subcat.Risks.Where(r => r != null))
                                        if (risk.MediaObjects != null)
                                            foreach (var media in risk.MediaObjects.Where(o => !string.IsNullOrWhiteSpace(o.Name) && o.Bytes != null && o.TypeMedia == Enums.TypeMedia.Image))
                                                mediaWithImagesPaths.Add(media);
                    }
                    
                    //Save unique images (by path from the server)
                    foreach (var image in mediaWithImagesPaths.GroupBy(m => m.Name).Select(grp => grp.First()))
                    {
                        var documentsPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
                        var localFilename = image.Name;
                        var localPath = System.IO.Path.Combine(documentsPath, localFilename);
                        using (FileStream fs = new FileStream(localPath, FileMode.OpenOrCreate))
                        {
                            await fs.WriteAsync(image.Bytes, 0, image.Bytes.Length);
                        }
                    }

                }
            }
            catch { }

        }
    }



}