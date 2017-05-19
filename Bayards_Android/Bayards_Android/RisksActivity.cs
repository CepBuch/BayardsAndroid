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
using static Android.Text.TextUtils;

namespace Bayards_Android
{
    [Activity(Theme = "@style/Theme.AppCompat.Light.NoActionBar")]
    public class RisksActivity : ActionBarActivity
    {
        ISharedPreferences prefs;
        RisksList risksList;
        ViewPager viewPager;
        RadioGroup subcategoriesTabs;
        TabLayout risksDottedTab;
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

            //Getting all subcategories and risks of parent category
            var subcategories = Database.Manager.GetSubcategories(parent_category_id, language);
            var risks = Database.Manager.GetRisks(parent_category_id, language);

            subcategoriesTabs = FindViewById<RadioGroup>(Resource.Id.tabsGroup);


            viewPager = FindViewById<ViewPager>(Resource.Id.viewpager);
            risksDottedTab = FindViewById<TabLayout>(Resource.Id.tabs_dots);
            risksDottedTab.SetupWithViewPager(viewPager);

            //Adding tabs of each subcategory and risks
            ShowCategoryContent(risks, subcategories);






            Android.Support.V7.Widget.Toolbar toolbar =
               FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar_risks);
            TextView toolbarTitle = FindViewById<TextView>(Resource.Id.toolbar_title);
            toolbarTitle.Selected = true;
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


        private void ShowCategoryContent(List<Risk> risks, List<Category> subCategories)
        {
            //First adding tab and showing risks of parent category if it has risks
            bool hasRisks = risks != null && risks.Count > 0;

            if (hasRisks)
            {
                AddTab("Risks", risks, hasRisks);
                ShowRisks(risks);
            }



            if (subCategories != null && subCategories.Count > 0)
            {
                subcategoriesTabs.Visibility = ViewStates.Visible;
                bool firstSubcategory = true;
                //Then adding tabs of all subcategories
                foreach (var subCat in subCategories)
                {
                    
                    var risksOfSubcat = Database.Manager.GetRisks(subCat.Id, language);
                    if (risksOfSubcat != null)
                        AddTab(subCat.Name, risksOfSubcat, !hasRisks && firstSubcategory);
                    firstSubcategory = false;
                }
            }
            else subcategoriesTabs.Visibility = ViewStates.Gone;


        }
        private void ShowRisks(List<Risk> risks)
        {
            if (risks == null || risks.Count == 0)
            {
                risks = new List<Risk>
                {
                    new Risk {Name = "" , Content = "There is no risks in this category"}
                };
            }
            risksDottedTab.Visibility = risks == null || risks.Count == 0 ||
                (risks != null && risks.Count > 1) ? ViewStates.Visible : ViewStates.Gone;
            risksList = new RisksList(risks);
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


        public void AddTab(string content, List<Risk> risks, bool isChecked = false)
        {

            var width = DpToPx(170);
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
            rb.Ellipsize = TruncateAt.Marquee;
            rb.SetMarqueeRepeatLimit(-1);
            rb.Selected = true;
            rb.SetSingleLine(true);

            rb.Click += (e, s) => ShowRisks(risks);

            if (isChecked)
                rb.Checked = true;



            subcategoriesTabs.AddView(rb, params_rb);
        }


        private int DpToPx(int dp)
        {
            var scale = Resources.DisplayMetrics.Density;
            return (int)((dp * scale) + 0.5);
        }

    }
}