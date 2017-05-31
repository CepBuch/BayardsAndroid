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
using Android.Util;
using Android.Support.V4.View;
using Bayards_Android.RiskViewModel;
using Android.Support.Design.Widget;
using Bayards_Android.CategoryViewModel;
using Bayards_Android.Model;
using Android.Preferences;
using SupportToolbar = Android.Support.V7.Widget.Toolbar;

namespace Bayards_Android
{
    [Activity(Theme = "@style/AppTheme")]
    public class CategoryContentActivity : AppCompatActivity
    {
        SupportToolbar toolbar;
        ViewPager viewPager;
        ISharedPreferences prefs;
        string language;
        protected override void OnCreate(Bundle savedInstanceState)
        {

            base.OnCreate(savedInstanceState);
            prefs = PreferenceManager.GetDefaultSharedPreferences(ApplicationContext);
            language = prefs.GetString("languageCode", "eng");

            SetContentView(Resource.Layout.CategoryContentActivity);

            //Getting Category id from previous activity.
            var parent_category_id = Intent.GetStringExtra("category_id");



            //Showing content pages
            viewPager = FindViewById<ViewPager>(Resource.Id.viewPager);
            viewPager.Adapter = new CategoryContentPagerAdapter(SupportFragmentManager, parent_category_id);

            TabLayout tabLayout = FindViewById<TabLayout>(Resource.Id.tabLayout);
            tabLayout.SetupWithViewPager(viewPager);      


            //Enabling custom toolbar
            toolbar =FindViewById<SupportToolbar>(Resource.Id.toolbar_risks);
            TextView toolbarTitle = FindViewById<TextView>(Resource.Id.toolbar_title);
            SetSupportActionBar(toolbar);

            //Disabling default title;
            SupportActionBar.SetDisplayShowTitleEnabled(false);

            //Showing name of parent_category as toolbar title
            var parent_category_name = Intent.GetStringExtra("category_name");
            if (!string.IsNullOrWhiteSpace(parent_category_name))
                toolbarTitle.Text = parent_category_name;

            //Enabling BackButton
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            SupportActionBar.SetDisplayShowHomeEnabled(true);


            RemoveEmptyTabs(parent_category_id);

        }

        public void RemoveEmptyTabs(string parent_category_id)
        {
            //Getting number of subcategories and risks for this category
            var numSubcategories = Database.Manager.CountSubcategories(parent_category_id, language);
            var numRisks = Database.Manager.CountRisks(parent_category_id, language);

            bool removedTasksTab = false;
            bool removedRisksTab = false;

            //Remove empty tabs (if this category doesn't have risks or subcategories
            if (!numSubcategories.HasValue || numSubcategories.Value < 1)
            {
                (viewPager.Adapter as CategoryContentPagerAdapter).RemovePage("Tasks");
                removedTasksTab = true;
                
            }

            if (!numRisks.HasValue || numRisks.Value < 1)
            {
                (viewPager.Adapter as CategoryContentPagerAdapter).RemovePage("Risks");
                removedRisksTab = true;

            }


            //Show message if both subcategories and risks tabs were removed
            //Then finish activity
            if (removedTasksTab && removedRisksTab)
            {
                var dialog = new Android.App.AlertDialog.Builder(this);
                string message = GetString(Resource.String.content_not_found);
                dialog.SetMessage(message);
                dialog.SetPositiveButton("Ok", delegate
                {
                    this.Finish();
                });
                dialog.Show();
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
    }
}