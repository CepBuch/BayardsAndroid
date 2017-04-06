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

namespace Bayards_Android
{
    [Activity(Theme = "@style/Theme.AppCompat.Light.NoActionBar")]
    public class RisksActivity : ActionBarActivity
    {
        CategoriesList categories;

        ViewPager viewPager;
        RadioGroup tabs;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            categories = new CategoriesList();
            //Предыдущее окно должно прислать название категории
            base.OnCreate(savedInstanceState);

            var category_id = Intent.GetIntExtra("category_id", -1);
            SetContentView(Resource.Layout.RisksLayout);


            tabs = FindViewById<RadioGroup>(Resource.Id.tabsGroup);
            AddRadioButton("Overall",true);
            AddRadioButton("SubCat1");
            AddRadioButton("SubCat2");
            AddRadioButton("SubCat3");
            AddRadioButton("SubCat4");




            //-----------------------------------------
            //viewPager = FindViewById<ViewPager>(Resource.Id.viewpager);

            //----------------------------------------





            Android.Support.V7.Widget.Toolbar toolbar =
               FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar_risks);
            TextView toolbarTitle = FindViewById<TextView>(Resource.Id.toolbar_title);
            SetSupportActionBar(toolbar);
            //Disabling default title and showing title from resources
            SupportActionBar.SetDisplayShowTitleEnabled(false);
            toolbarTitle.Text = categories[category_id].Name;

            //BackButton
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            SupportActionBar.SetDisplayShowHomeEnabled(true);


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


        public void AddRadioButton(string content,bool is_checked = false)
        {

            var width = DpToPx(120);
            var height = DpToPx(40);

            RadioGroup.LayoutParams params_rb
                = new RadioGroup.LayoutParams(width, height);
            params_rb.SetMargins(DpToPx(15), 0, DpToPx(15), 0);
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
            tabs.AddView(rb, params_rb);

        }

        private int DpToPx(int dp)
        {
            var scale = Resources.DisplayMetrics.Density;
            return (int)((dp * scale) + 0.5);

        }

    }
}