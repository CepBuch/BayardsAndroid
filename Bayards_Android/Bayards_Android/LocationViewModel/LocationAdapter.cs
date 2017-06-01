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
using Android.Support.V7.Widget;

namespace Bayards_Android.LocationViewModel
{
    class LocationAdapter : RecyclerView.Adapter
    {
        public LocationList _locationList;

        public LocationAdapter(LocationList locations)
        {
            _locationList = locations;
        }
        public override int ItemCount
        {
            get { return _locationList.NumLocations; }
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            RecyclerView.ViewHolder rh;
            View itemView = LayoutInflater.From(parent.Context).
                           Inflate(Resource.Layout.LocationView, parent, false);
            rh = new LocationViewHolder(itemView);
            return rh;
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            //Modifying base view with exact data
            if (holder is LocationViewHolder)
            {
                LocationViewHolder lh = holder as LocationViewHolder;
                lh.NameTextView.Text = _locationList[position].Name;
                lh.ContentTextView.Text = _locationList[position].Content;
                lh.PositionTextView.Text += $": ({_locationList[position].Latitude}; {_locationList[position].Longtitude})";
            }
        }


    }
}