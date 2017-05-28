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

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.PasswordActivity);


            _prefs = PreferenceManager.GetDefaultSharedPreferences(ApplicationContext);
            _editor = _prefs.Edit();



            passwordBox = FindViewById<EditText>(Resource.Id.password_text);
            contButton = FindViewById<Button>(Resource.Id.continuePasswordButton);
            warningLayout = FindViewById<LinearLayout>(Resource.Id.warningLayout);
            waitLayout = FindViewById<LinearLayout>(Resource.Id.waitLayout);

            contButton.Click += ContButton_Click;

            //Deleting "incorrect password" message when user starts to enter password (if prev. attempt was fail)
            //Enabling button only when password box is not empty
            passwordBox.TextChanged += delegate
            {
                warningLayout.Visibility = ViewStates.Gone;
                contButton.Enabled = !string.IsNullOrWhiteSpace(passwordBox.Text);
            };


        }

        private async void ContButton_Click(object sender, EventArgs e)
        {
            //Show "wait" message and disable controls while wating response from a server
            warningLayout.Visibility = ViewStates.Gone;
            waitLayout.Visibility = ViewStates.Visible;
            contButton.Enabled = false;
            passwordBox.Enabled = false;


            //get response from a server
            bool correctPassword = false;
            string encrypted_password = CreditnailsConverter.ConvertToMD5(passwordBox.Text);
            try
            {
                ApiProvider provider = new ApiProvider(_prefs.GetString("hosting_address", ""));
                correctPassword = await provider.CheckPassword(encrypted_password);
            }
            catch
            {
                var dialog = new Android.App.AlertDialog.Builder(this);
                string message = "There are problems with connecting to the server. Please, check your internet connection.";
                dialog.SetMessage(message);
                dialog.SetPositiveButton("Ok", delegate { });
                dialog.Show();
            };

            //Remove "wait" message and enable button
            contButton.Enabled = true;
            passwordBox.Enabled = true;
            passwordBox.Text = string.Empty;
            waitLayout.Visibility = ViewStates.Gone;


            //Checking if password is correct, otherwise show "incorrect password" message
            if (correctPassword)
            {
                //Remember that user is authorized and open main page
                _editor.PutBoolean("isAuthorized", true);
                _editor.PutString("lastPassword", encrypted_password);

                _editor.Apply();
                var intent = new Intent(this, typeof(AgreementActivity));
                StartActivity(intent);
                this.Finish();
            }
            else
            {
                warningLayout.Visibility = ViewStates.Visible;
            }
        }
    }
}