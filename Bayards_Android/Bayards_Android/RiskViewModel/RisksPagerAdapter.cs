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
    class RisksPagerAdapter: FragmentStatePagerAdapter
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
            return (Android.Support.V4.App.Fragment)RiskContentFragment.newInstance(risks[position]);
        }

    }
}