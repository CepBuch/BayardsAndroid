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
using Android.Support.V4.App;
using Java.Lang;
using Bayards_Android.Fragments;

namespace Bayards_Android.LocationViewModel
{
    class LocationPagerAdapter : FragmentStatePagerAdapter
    {
        List<Android.Support.V4.App.Fragment> fragments;
        List<string> titles;

        public LocationPagerAdapter(Android.Support.V4.App.FragmentManager fm, List<string> titles)
            : base(fm)
        {
            fragments = new List<Android.Support.V4.App.Fragment>
            {
                LocationsContainerFragment.newInstance(),
                LocationsContainerFragment.newInstance()
            };
            this.titles = titles;
        }


        public override Android.Support.V4.App.Fragment GetItem(int position)
        {
            return fragments[position];
        }

        public override ICharSequence GetPageTitleFormatted(int position)
        {
            return new Java.Lang.String(titles[position]);
        }

        public override int Count
        {
            get
            {
                return fragments.Count;
            }
        }

    }
}