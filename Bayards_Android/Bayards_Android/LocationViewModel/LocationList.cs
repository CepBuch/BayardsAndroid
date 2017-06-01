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
using Android.Locations;

namespace Bayards_Android.LocationViewModel
{
    public class LocationList
    {
        public List<Model.Location> Locations { get; private set; }

        public LocationList(List<Model.Location> locations)
        {
            Locations = locations;
        }

        public int NumLocations
        {
            get { return Locations.Count; }
        }
        public Model.Location this[int i]
        {
            get { return Locations[i]; }
        }
    }
}