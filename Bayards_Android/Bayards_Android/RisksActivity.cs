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

namespace Bayards_Android
{
    [Activity(Theme = "@style/Theme.AppCompat.Light.NoActionBar")]
    public class RisksActivity : ActionBarActivity
    {
        ISharedPreferences prefs;
        RisksList risksList;
        ViewPager viewPager;
        RadioGroup tabs;
        protected override void OnCreate(Bundle savedInstanceState)
        {

            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.RisksLayout);


            prefs = PreferenceManager.GetDefaultSharedPreferences(ApplicationContext);

            //Getting Category id from previous activity.
            var parent_category_id = Intent.GetStringExtra("category_id");

            //Getting current lunguage from application properties. 
            var language = prefs.GetString("languageCode", "eng");
    
            //Getting all subcategories and risks of parent category
            var subcategories = Database.Manager.GetSubcategories(parent_category_id, language);

            var risks = Database.Manager.GetRisks(parent_category_id, language);

            tabs = FindViewById<RadioGroup>(Resource.Id.tabsGroup);

            //Adding tabs of each subcategory and risks
            ShowCategoryContent(risks, subcategories);

            viewPager = FindViewById<ViewPager>(Resource.Id.viewpager);
            TabLayout tabLayout = FindViewById<TabLayout>(Resource.Id.tabs_dots);
            tabLayout.SetupWithViewPager(viewPager);

            


            Android.Support.V7.Widget.Toolbar toolbar =
               FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar_risks);
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


        private void ShowCategoryContent(IEnumerable<Risk> risks, IEnumerable<Category> subCategories)
        {
            //First adding tab and showing risks of parent category
            AddTab("Risks", risks);


            ShowRisks(risks);


            //Then adding tabs of all subcategories
            foreach (var subCat in subCategories)
            {
                AddTab(subCat.Name, subCat.Risks);
            }
            
            
        }
        private void ShowRisks(IEnumerable<Risk> risks)
        {
            ApiProvider api = new ApiProvider();
            risksList = new RisksList(/*api.GetRisks(a)*/ null);
            viewPager.Adapter = new RisksPagerAdapter(SupportFragmentManager, risksList);
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


        public void AddTab(string content, IEnumerable<Risk> risks, bool isChecked  = false)
        {

            var width = DpToPx(120);
            var height = DpToPx(40);

            RadioGroup.LayoutParams params_rb
                = new RadioGroup.LayoutParams(width, height);
            params_rb.SetMargins(DpToPx(7), 0, DpToPx(7), 0);
            RadioButton rb = new RadioButton(this)
            {
                Gravity = GravityFlags.Center,
                Id = View.GenerateViewId()
            };
            rb.SetText(content, TextView.BufferType.Normal);
            rb.SetTextSize(Android.Util.ComplexUnitType.Sp, 15);
            rb.SetTextColor(GetColorStateList(Resource.Drawable.radiobutton_text));
            rb.SetBackgroundResource(Resource.Drawable.radiobutton_shape);
            rb.SetButtonDrawable(Android.Resource.Color.Transparent);


            if (isChecked)
                rb.Checked = true;

            rb.Click += (e, s) => ShowRisks(risks);


            tabs.AddView(rb, params_rb);
        }

        private int DpToPx(int dp)
        {
            var scale = Resources.DisplayMetrics.Density;
            return (int)((dp * scale) + 0.5);
        }

    }
}