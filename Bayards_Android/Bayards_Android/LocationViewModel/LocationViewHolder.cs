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
    class LocationViewHolder : RecyclerView.ViewHolder
    {
        public TextView NameTextView { get; set; }

        public TextView ContentTextView { get; set; }

        public TextView PositionTextView { get; set; }

        private Button ShowButton { get; set; }
        //Defining view of each RecycleView item
        public LocationViewHolder(View itemView, Action<int> listener)
            :base(itemView)
        {
            NameTextView = itemView.FindViewById<TextView>(Resource.Id.location_name);
            ContentTextView = itemView.FindViewById<TextView>(Resource.Id.location_content);
            PositionTextView = itemView.FindViewById<TextView>(Resource.Id.location_position);
            ShowButton = itemView.FindViewById<Button>(Resource.Id.showLocationButton);
            ShowButton.Click += (sender, e) => listener(base.AdapterPosition);
        }
    }
}