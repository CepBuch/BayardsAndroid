using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Android.Support.V7.Widget;
using Bayards_Android.LocationViewModel;
using Android.Preferences;
using Android.Support.V4.View;
using Android.Support.Design.Widget;

namespace Bayards_Android.Fragments
{
    public class LocationsFragment : Android.Support.V4.App.Fragment
    {
        ViewPager viewPager;

        public static LocationsFragment newInstance()
        {
            LocationsFragment fragment = new LocationsFragment();
            Bundle args = new Bundle();
            fragment.Arguments = args;
            return fragment;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View view = inflater.Inflate(Resource.Layout.LocationsFragment, container, false);
            var titles = new List<string>
            {
                "Map",
                GetString(Resource.String.nav_map)
             };
            //Showing content pages
            viewPager = view.FindViewById<ViewPager>(Resource.Id.viewPager);
            viewPager.Adapter = new LocationPagerAdapter(FragmentManager, titles);
            TabLayout tabLayout = view.FindViewById<TabLayout>(Resource.Id.tabLayout);
            tabLayout.SetupWithViewPager(viewPager);
            return view;
        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

    }
}