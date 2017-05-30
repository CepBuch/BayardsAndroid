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

namespace Bayards_Android.RiskViewModel
{
    class RiskViewHolder : RecyclerView.ViewHolder
    {
        public TextView NameTextView { get; set; }

        public TextView ContentTextView { get; set; }
        //Defining view of each RecycleView item
        public RiskViewHolder(View itemView)
            :base(itemView)
        {
            NameTextView = itemView.FindViewById<TextView>(Resource.Id.risk_name);
            ContentTextView = itemView.FindViewById<TextView>(Resource.Id.risk_content);

        }
    }
}