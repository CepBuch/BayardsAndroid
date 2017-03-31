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

namespace Bayards_Android
{
    [Activity(Label = "PasswordActivity", Theme = "@android:style/Theme.DeviceDefault.Light.NoActionBar")]
    public class PasswordActivity : Activity
    {
        EditText passwordBox;
        Button contButton;
        LinearLayout warningLayout;
        LinearLayout waitLayout;

        Repository _repo = new Repository();

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.PasswordLayout);

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
            var correctPassword = await _repo.sendPassword(passwordBox.Text);

            //Remove "wait" message and enable button
            contButton.Enabled = true;
            passwordBox.Enabled = true;
            passwordBox.Text = string.Empty;
            waitLayout.Visibility = ViewStates.Gone;

            //Checking if password is correct, otherwise show "incorrect password" message
            if (correctPassword)
            {
                var intent = new Intent(this, typeof(AgreementActivity));
                StartActivity(intent);
            }
            else warningLayout.Visibility = ViewStates.Visible;
        }
    }
}