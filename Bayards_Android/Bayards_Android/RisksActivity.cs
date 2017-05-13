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

namespace Bayards_Android
{
    [Activity(Theme = "@style/Theme.AppCompat.Light.NoActionBar")]
    public class RisksActivity : ActionBarActivity
    {
        //CategoriesList categories;
        RisksList risksList;
        ViewPager viewPager;
        RadioGroup tabs;
        protected override void OnCreate(Bundle savedInstanceState)
        {

            base.OnCreate(savedInstanceState);

            var category_id = Intent.GetStringExtra("category_id");

            Category cat = new Category();

            if (category_id != null)
                cat = Database.Manager.GetCategory(category_id);

            SetContentView(Resource.Layout.RisksLayout);


            tabs = FindViewById<RadioGroup>(Resource.Id.tabsGroup);
            AddTab("Overall", true);
            AddTab("SubCat1");
            AddTab("SubCat2");

            viewPager = FindViewById<ViewPager>(Resource.Id.viewpager);
            TabLayout tabLayout = FindViewById<TabLayout>(Resource.Id.tabs_dots);
            tabLayout.SetupWithViewPager(viewPager);

            ShowRisks(true);


            Android.Support.V7.Widget.Toolbar toolbar =
               FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar_risks);
            TextView toolbarTitle = FindViewById<TextView>(Resource.Id.toolbar_title);
            SetSupportActionBar(toolbar);
            //Disabling default title and showing title 
            SupportActionBar.SetDisplayShowTitleEnabled(false);

            if (cat != null)
                toolbarTitle.Text = cat.Name;

            //BackButton
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            SupportActionBar.SetDisplayShowHomeEnabled(true);


        }

        private void ShowRisks(bool a)
        {
            ApiProvider api = new ApiProvider();
            risksList = new RisksList(api.GetRisks(a));
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


        public void AddTab(string content, bool is_checked = false)
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


            if (is_checked)
                rb.Checked = true;

            rb.Click += (e, s) =>
            {
                Toast.MakeText(this, content + " clicked", ToastLength.Long).Show();
                ShowRisks(content == "Overall");
            };


            tabs.AddView(rb, params_rb);

        }

        private int DpToPx(int dp)
        {
            var scale = Resources.DisplayMetrics.Density;
            return (int)((dp * scale) + 0.5);

        }

    }
}