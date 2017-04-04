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

namespace Bayards_Android
{
    [Activity(Theme = "@style/Theme.AppCompat.Light.NoActionBar")]
    public class RisksActivity : ActionBarActivity
    {
        CategoriesList categories;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            categories = new CategoriesList();
            //Предыдущее окно должно прислать название категории
            base.OnCreate(savedInstanceState);

            var category_id = Intent.GetIntExtra("category_id", -1);
            SetContentView(Resource.Layout.RisksLayout);



            Android.Support.V7.Widget.Toolbar toolbar =
               FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar_risks);
            TextView toolbarTitle = FindViewById<TextView>(Resource.Id.toolbar_title);
            SetSupportActionBar(toolbar);
            //Disabling default title and showing title from resources
            SupportActionBar.SetDisplayShowTitleEnabled(false);
            toolbarTitle.Text = categories[category_id];

            
        }
    }
}