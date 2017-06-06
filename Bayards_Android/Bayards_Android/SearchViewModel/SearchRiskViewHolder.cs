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

namespace Bayards_Android.SearchViewModel
{
    class SearchRiskViewHolder : RecyclerView.ViewHolder
    {
        public Button ContentButton { get; private set; }

        //Defining view of each RecycleView item
        public SearchRiskViewHolder(View itemView, Action<int> listener)
            : base(itemView)
        {
            ContentButton = itemView.FindViewById<Button>(Resource.Id.riskButton);
            ContentButton.Selected = true;
            ContentButton.Click += (sender, e) => listener(base.AdapterPosition);
        }
    }

    class SearchHeaderViewHolder : RecyclerView.ViewHolder
    {
        public TextView TextView { get; private set; }

        public SearchHeaderViewHolder(View itemView) : base(itemView)
        {
            TextView = itemView.FindViewById<TextView>(Resource.Id.headerTextView);
        }
    }
}