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

        Repository _repo = new Repository();

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.PasswordLayout);

            passwordBox = FindViewById<EditText>(Resource.Id.password_text);
            contButton = FindViewById<Button>(Resource.Id.continuePasswordButton);
            warningLayout = FindViewById<LinearLayout>(Resource.Id.warningLayout);


            contButton.Click += ContButton_Click;

            passwordBox.TextChanged += delegate
            {
                warningLayout.Visibility = ViewStates.Invisible;
            };


        }

        private async void ContButton_Click(object sender, EventArgs e)
        {
                if (await _repo.sendPassword(passwordBox.Text) == true)
                {
                    warningLayout.Visibility = ViewStates.Invisible;
                    var intent = new Intent(this, typeof(AgreementActivity));
                    StartActivity(intent);
                }
                else
                {
                    passwordBox.Text = string.Empty;
                    warningLayout.Visibility = ViewStates.Visible;
                }
        }
    }
}