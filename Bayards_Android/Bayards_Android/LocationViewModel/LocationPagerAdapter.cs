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
        public event Action<int> OnTabChage;

        public LocationPagerAdapter(Android.Support.V4.App.FragmentManager fm, List<string> titles, Context context)
            : base(fm)
        {

            var mapFragment = Fragments.MapFragment.newInstance();
            var containerFragment = LocationsContainerFragment.newInstance();
            containerFragment.ItemClick += (e, l) =>
            {
                OnTabChage?.Invoke(0);
                mapFragment.ShowLocation(l);
            };
            
            fragments = new List<Android.Support.V4.App.Fragment>
            {
                mapFragment,
                containerFragment
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