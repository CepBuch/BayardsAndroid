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
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using Android.Preferences;

namespace Bayards_Android.Fragments
{
    public class MapFragment : Android.Support.V4.App.Fragment, IOnMapReadyCallback
    {
        MapView mapView;
        private GoogleMap googleMap;
        ISharedPreferences prefs;
        List<Marker> markers;


        public static MapFragment newInstance()
        {
            MapFragment fragment = new MapFragment();
            Bundle args = new Bundle();
            fragment.Arguments = args;
            return fragment;
        }
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View view = inflater.Inflate(Resource.Layout.MapFragment, container, false);
            mapView = view.FindViewById<MapView>(Resource.Id.mapView);

            mapView.OnCreate(savedInstanceState);
            mapView.OnResume();

            try
            {
                MapsInitializer.Initialize(Activity.ApplicationContext);
            }
            catch { }

            mapView.GetMapAsync(this);

            return view;
        }
        public override void OnCreate(Bundle savedInstanceState)
        {
            prefs = PreferenceManager.GetDefaultSharedPreferences(Activity.ApplicationContext);

            base.OnCreate(savedInstanceState);
        }

        public void ShowLocation(Model.Location location)
        {
            var thisLocationMarker = markers.FirstOrDefault(m => m.Position.Latitude == location.Latitude &&
            m.Position.Longitude == location.Longtitude);

            if(thisLocationMarker != null)
            {
                thisLocationMarker.ShowInfoWindow();
            }
            googleMap.AnimateCamera(CameraUpdateFactory.NewLatLngZoom(
                new LatLng(location.Latitude, location.Longtitude), 14));
        }

        public void OnMapReady(GoogleMap googleMap)
        {
            this.googleMap = googleMap;
            googleMap.UiSettings.ZoomControlsEnabled = true;
            string mapType = prefs.GetString("mapType", "norm");

            switch (mapType)
            {
                case "norm":
                    googleMap.MapType = GoogleMap.MapTypeNormal;
                    break;
                case "sat":
                    googleMap.MapType = GoogleMap.MapTypeSatellite;
                    break;
                case "hyb":
                    googleMap.MapType = GoogleMap.MapTypeHybrid;
                    break;
                default:
                    goto case "norm";
            }
            // For showing a move to my location button
            googleMap.MyLocationEnabled = true;
            AddMarkers();
            googleMap.AnimateCamera(CameraUpdateFactory.NewLatLngZoom(
                new LatLng(52.132633, 5.291266), 2));
        }


        private void AddMarkers()
        {
            markers = new List<Marker>();

            if (googleMap != null)
            {
                List<Model.Location> locations = new List<Model.Location>();
                string language = prefs.GetString("languageCode", "eng");
                locations = Database.Manager.GetLocations(language);
                if (locations != null && locations.Count > 0)
                {
                    foreach (var loc in locations)
                    {
                        MarkerOptions markerOptions = new MarkerOptions();
                        markerOptions.SetPosition(new LatLng(loc.Latitude, loc.Longtitude));
                        markerOptions.SetTitle(loc.Name);
                        var locationMarker = googleMap.AddMarker(markerOptions);
                        markers.Add(locationMarker);
                    }
                }
            }
        }
    }
}