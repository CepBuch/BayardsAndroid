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
using Android.Support.V7.App;
using Android.Support.V4.View;
using Android.Preferences;

namespace Bayards_Android
{
    [Activity(Theme = "@style/Theme.AppCompat.Light.NoActionBar")]
    public class AgreementActivity : ActionBarActivity
    {
        TextView userText;
        Switch agreeSwitch;
        Button contButton;

        ISharedPreferences _prefs;
        ISharedPreferencesEditor _editor;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.AgreementLayout);


            _prefs = PreferenceManager.GetDefaultSharedPreferences(ApplicationContext);
            _editor = _prefs.Edit();


            //Accepting custom toolbar 
            Android.Support.V7.Widget.Toolbar toolbar = 
                FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            TextView toolbarTitle = FindViewById<TextView>(Resource.Id.toolbar_title);
            SetSupportActionBar(toolbar);
            //Disabling default title and showing title from resources
            SupportActionBar.SetDisplayShowTitleEnabled(false);
            toolbarTitle.Text = Resources.GetString(Resource.String.bayards);


            userText = FindViewById<TextView>(Resource.Id.useragreementText);
            agreeSwitch = FindViewById<Switch>(Resource.Id.agreement_switch);
            contButton = FindViewById<Button>(Resource.Id.continueUserButton);
            userText.Text = getLorem(3);

            agreeSwitch.CheckedChange += AgreeSwitch_CheckedChange;
            contButton.Click += ContButton_Click;
        }

        private void ContButton_Click(object sender, EventArgs e)
        {
            var intent = new Intent(this,typeof(MainActivity));
            StartActivity(intent);

            //Remember that agreement is accepted.
            _editor.PutBoolean("isAcceptedAgreement", true);
            _editor.Apply();


            this.Finish();
        }

        private void AgreeSwitch_CheckedChange(object sender, CompoundButton.CheckedChangeEventArgs e)
        {
            contButton.Enabled = agreeSwitch.Checked;
        }

        //Temporary method
        public string getLorem(int times)
        {
            string result = string.Empty;
            for (int i = 0; i <= times; i++)
            {
                result += Resources.GetString(Resource.String.lorem);
            }
            return result;
        }

    }
}