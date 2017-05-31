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
using Android.Net;
using System.Threading.Tasks;

namespace Bayards_Android
{
    [Activity(Label = "PasswordActivity",
        Theme = "@android:style/Theme.DeviceDefault.Light.NoActionBar")]
    public class PasswordActivity : Activity
    {
        EditText passwordBox;
        Button contButton;
        LinearLayout warningLayout, waitLayout;
        ISharedPreferences _prefs;
        ISharedPreferencesEditor _editor;
        ApiProvider provider;
        string language;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.PasswordActivity);

            _prefs = PreferenceManager.GetDefaultSharedPreferences(ApplicationContext);
            _editor = _prefs.Edit();
            language = _prefs.GetString("languageCode", "eng");

            FindViews();

            contButton.Click += (e, s) => CheckConnection();
            passwordBox.TextChanged += PasswordBox_TextChanged;
        }

        private void FindViews()
        {
            passwordBox = FindViewById<EditText>(Resource.Id.password_text);
            contButton = FindViewById<Button>(Resource.Id.continuePasswordButton);
            warningLayout = FindViewById<LinearLayout>(Resource.Id.warningLayout);
            waitLayout = FindViewById<LinearLayout>(Resource.Id.waitLayout);
        }

        private async Task CheckPassword()
        {
            //Show "wait" message and disable controls while wating response from a server
            warningLayout.Visibility = ViewStates.Gone;
            waitLayout.Visibility = ViewStates.Visible;
            contButton.Enabled = false;
            passwordBox.Enabled = false;


            bool correctPassword = false;
            string encrypted_password = CreditnailsConverter.ConvertToMD5(passwordBox.Text);


            var hosting_address = _prefs.GetString("hosting_address", "");

            provider = new ApiProvider(hosting_address, encrypted_password);


            try
            {
                correctPassword = await provider.CheckPassword(encrypted_password);
            }
            catch
            {
                var dialog = new Android.App.AlertDialog.Builder(this);
                string message = GetString(Resource.String.connection_problems);
                dialog.SetMessage(message);
                dialog.SetPositiveButton("Ok", delegate { });
                dialog.Show();
                correctPassword = false;
            };

            //Remove "wait" message and enable button
            contButton.Enabled = true;
            passwordBox.Enabled = true;
            passwordBox.Text = string.Empty;
            waitLayout.Visibility = ViewStates.Gone;


            //Checking if password is correct, otherwise show "incorrect password" message
            if (correctPassword)
            {
                await DownloadAgreement();
                //Remember that user is authorized and open main page
                _editor.PutBoolean("isAuthorized", true);
                _editor.PutString("token", encrypted_password);
                _editor.Apply();

                //Download user agreement to show it on the next page
                


                var intent = new Intent(this, typeof(MainActivity));
                StartActivity(intent);
                this.Finish();
            }
            else
            {
                warningLayout.Visibility = ViewStates.Visible;
            }
        }

        private async Task DownloadAgreement()
        {
            try
            {
                var agreement = await provider.GetUserAgreement(language);
                _editor.PutString("userAgreement", agreement);
            }
            catch { }
           
        }

        private void PasswordBox_TextChanged(object sender, Android.Text.TextChangedEventArgs e)
        {
            //Deleting "incorrect password" message when user starts to enter password (if prev. attempt was fail)
            //Enabling button only when password box is not empty
            warningLayout.Visibility = ViewStates.Gone;
            contButton.Enabled = !string.IsNullOrWhiteSpace(passwordBox.Text);
        }

        private async void CheckConnection()
        {
            ConnectivityManager connectivityManager = (ConnectivityManager)GetSystemService(ConnectivityService);
            NetworkInfo info = connectivityManager.ActiveNetworkInfo;


            //If connected to the internet
            if (info != null && info.IsConnected)
            {
                await CheckPassword();
            }
            else
            {
                var dialog = new Android.App.AlertDialog.Builder(this);
                //Show error message
                string message = GetString(Resource.String.password_connection);
                dialog.SetMessage(message);
                dialog.SetPositiveButton("Ok", delegate { });
                dialog.Show();
                passwordBox.Text = string.Empty;
            }
        }
    }



}