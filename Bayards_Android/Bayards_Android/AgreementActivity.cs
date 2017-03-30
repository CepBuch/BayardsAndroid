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

namespace Bayards_Android
{
    [Activity(Theme = "@style/Theme.AppCompat.Light.NoActionBar")]
    public class AgreementActivity : ActionBarActivity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.UserAgreementScreen);

            //Accepting custom toolbar 
            Android.Support.V7.Widget.Toolbar toolbar = 
                FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            TextView toolbarTitle = FindViewById<TextView>(Resource.Id.toolbar_title);
            SetSupportActionBar(toolbar);
            //Disabling default title and showing title from resources
            SupportActionBar.SetDisplayShowTitleEnabled(false);
            toolbarTitle.Text = Resources.GetString(Resource.String.bayards);
            TextView userView = FindViewById<TextView>(Resource.Id.useragreementView);


            userView.Text = getLorem(3);

        }

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