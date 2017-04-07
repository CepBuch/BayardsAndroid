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
using Android.Support.V4.View;
using Android.Support.V4.App;

namespace Bayards_Android.RiskViewModel
{
    class RisksPagerAdapter: FragmentPagerAdapter  /*PagerAdapter*/
    {
        public RisksList risks;
        public RisksPagerAdapter(Android.Support.V4.App.FragmentManager fm, RisksList risks)
            : base(fm)
        {
            this.risks = risks;
        }

        public override int Count
        {
            get { return risks.NumRisks; }
        }

        public override Android.Support.V4.App.Fragment GetItem(int position)
        {
            return (Android.Support.V4.App.Fragment)RiskInfoFragment.newInstance(risks[position].Name);
        }


        //Context context;
        //RisksList risks;

        //public RisksPagerAdapter(Context context, RisksList risks)
        //{
        //    this.context = context;
        //    this.risks = risks;
        //}

        //public override Java.Lang.Object InstantiateItem(View container, int position)
        //{
        //    var textView = new TextView(context);
        //    textView.Text = risks[position].Name;
        //    var viewPager = container.JavaCast<ViewPager>();
        //    viewPager.AddView(textView);
        //    return textView;
        //}

        //public override void DestroyItem(View container, int position, Java.Lang.Object view)
        //{
        //    var viewPager = container.JavaCast<ViewPager>();
        //    viewPager.RemoveView(view as View);
        //}


        //public override int Count
        //{
        //    get { return risks.NumRisks; }
        //}

        //public override bool IsViewFromObject(View view, Java.Lang.Object obj)
        //{
        //    return view == obj;
        //}
    }
}