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
using SupportToolbar = Android.Support.V7.Widget.Toolbar;
using Android.Support.V4.View;
using Android.Preferences;

namespace Bayards_Android
{
    [Activity(Theme = "@style/AppTheme")]
    public class AgreementActivity : AppCompatActivity
    {
        TextView userAgreementText;
        Switch agreeSwitch;
        Button contButton;
        SupportToolbar toolbar;
        ISharedPreferences _prefs;
        ISharedPreferencesEditor _editor;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.AgreementActivity);

            _prefs = PreferenceManager.GetDefaultSharedPreferences(ApplicationContext);
            _editor = _prefs.Edit();

            FindViews();
            CustomizeToolbar();

            //Adding eventhandlers
            userAgreementText.Text = _prefs.GetString("userAgreement", "Problems with loading user agreement");
            agreeSwitch.CheckedChange += AgreeSwitch_CheckedChange;
            contButton.Click += ContButton_Click;
        }


        public void FindViews()
        {
            userAgreementText = FindViewById<TextView>(Resource.Id.useragreementText);
            agreeSwitch = FindViewById<Switch>(Resource.Id.agreement_switch);
            contButton = FindViewById<Button>(Resource.Id.continueUserButton);
        }
        public void CustomizeToolbar()
        {
            //Accepting custom toolbar 
            toolbar = FindViewById<SupportToolbar>(Resource.Id.toolbar_agreement);
            TextView toolbarTitle = FindViewById<TextView>(Resource.Id.toolbar_title);
            SetSupportActionBar(toolbar);
            //Disabling default title and showing title from resources
            SupportActionBar.SetDisplayShowTitleEnabled(false);
            toolbarTitle.Text = Resources.GetString(Resource.String.bayards);
        }
        private void AgreeSwitch_CheckedChange(object sender, CompoundButton.CheckedChangeEventArgs e)
        {
            contButton.Enabled = agreeSwitch.Checked;
        }

        private void ContButton_Click(object sender, EventArgs e)
        {
            //Remember that agreement is accepted.
            _editor.PutBoolean("isAcceptedAgreement", true);
            _editor.Apply();

            var intent = new Intent(this, typeof(MainActivity));
            StartActivity(intent);
            this.Finish();
        }


    }
}