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

namespace Bayards_Android.Fragments
{
    public class LocationsContainerFragment : Android.Support.V4.App.Fragment
    {
        RecyclerView recyclerView;
        ISharedPreferences prefs;
        RecyclerView.LayoutManager layoutManager;
        LocationAdapter locationAdapter;
        LocationList locationList;


        public static LocationsContainerFragment newInstance()
        {
            LocationsContainerFragment fragment = new LocationsContainerFragment();
            Bundle args = new Bundle();
            fragment.Arguments = args;
            return fragment;
        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            prefs = PreferenceManager.GetDefaultSharedPreferences(Activity.ApplicationContext);
            //Getting current lunguage from application properties. 
            string language = prefs.GetString("languageCode", "eng");

            InitData(language);
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View view = inflater.Inflate(Resource.Layout.LocationsContainerFragment, container, false);

            recyclerView = view.FindViewById<RecyclerView>(Resource.Id.recycler_view);
            recyclerView.SetAdapter(locationAdapter);

            layoutManager = new LinearLayoutManager(Activity);
            recyclerView.SetLayoutManager(layoutManager);
            return view;
        }

        private void InitData(string language)
        {
            List<Model.Location> locations = new List<Model.Location>();
            locations = Database.Manager.GetLocations(language);

            if (locations != null && locations.Count > 0)
            {
                locationList = new LocationList(locations);
                locationAdapter = new LocationAdapter(locationList);
            }
        }
    }
}