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
    public class RisksActivity : ActionBarActivity
    {
        ISharedPreferences prefs;
        RisksList risksList;
        SupportToolbar toolbar;
        ViewPager viewPager;
        string language;
        protected override void OnCreate(Bundle savedInstanceState)
        {

            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.RisksLayout);


            prefs = PreferenceManager.GetDefaultSharedPreferences(ApplicationContext);

            

            //Getting Category id from previous activity.
            var parent_category_id = Intent.GetStringExtra("category_id");

            //Getting current lunguage from application properties. 
            language = prefs.GetString("languageCode", "eng");

            viewPager = FindViewById<ViewPager>(Resource.Id.viewPager);
            viewPager.Adapter = new CategoryContentPagerAdapter(SupportFragmentManager, parent_category_id);

            TabLayout tabLayout = FindViewById<TabLayout>(Resource.Id.tabLayout);
            tabLayout.SetupWithViewPager(viewPager);

            

            toolbar =FindViewById<SupportToolbar>(Resource.Id.toolbar_risks);
            TextView toolbarTitle = FindViewById<TextView>(Resource.Id.toolbar_title);
            SetSupportActionBar(toolbar);


            //Disabling default title and showing  custom (from .xml) title 
            SupportActionBar.SetDisplayShowTitleEnabled(false);

            //Showing name of parent_category as toolbar title
            var parent_category_name = Intent.GetStringExtra("category_name");
            if (!string.IsNullOrWhiteSpace(parent_category_name))
                toolbarTitle.Text = parent_category_name;

            //Enabling BackButton
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            SupportActionBar.SetDisplayShowHomeEnabled(true);


        }


        //private void ShowCategoryContent(List<Risk> risks, List<Category> subCategories)
        //{
        //    //First adding tab and showing risks of parent category if it has risks
        //    bool hasRisks = risks != null && risks.Count > 0;

        //    if (hasRisks)
        //    {
        //        AddTab("Risks", risks, hasRisks);
        //        ShowRisks(risks);
        //    }



        //    if (subCategories != null && subCategories.Count > 0)
        //    {
        //        subcategoriesTabs.Visibility = ViewStates.Visible;
        //        bool firstSubcategory = true;
        //        //Then adding tabs of all subcategories
        //        foreach (var subCat in subCategories)
        //        {

        //            var risksOfSubcat = Database.Manager.GetRisks(subCat.Id, language);
        //            if (risksOfSubcat != null)
        //                AddTab(subCat.Name, risksOfSubcat, !hasRisks && firstSubcategory);
        //            firstSubcategory = false;
        //        }
        //    }
        //    else subcategoriesTabs.Visibility = ViewStates.Gone;


        //}
        //private void ShowRisks(List<Risk> risks)
        //{
        //    if (risks == null || risks.Count == 0)
        //    {
        //        risks = new List<Risk>
        //        {
        //            new Risk {Name = "" , Content = "There is no risks in this category"}
        //        };
        //    }
        //    risksDottedTab.Visibility = risks == null || risks.Count == 0 ||
        //        (risks != null && risks.Count > 1) ? ViewStates.Visible : ViewStates.Gone;
        //    risksList = new RisksList(risks);
        //    viewPager.Adapter = new RisksPagerAdapter(SupportFragmentManager, risksList);
        //}




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


        //rb.Click += (e, s) => ShowRisks(risks);



    }
}