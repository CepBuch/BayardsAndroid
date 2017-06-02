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
        public Switch DoneSwitch;
        public TextView ContentTextView { get; set; }
        //Defining view of each RecycleView item
        public RiskViewHolder(View itemView, Action<int,int> listener)
            :base(itemView)
        {
            NameTextView = itemView.FindViewById<TextView>(Resource.Id.risk_name);
            ContentTextView = itemView.FindViewById<TextView>(Resource.Id.risk_content);
            DoneSwitch = itemView.FindViewById<Switch>(Resource.Id.done_switch);
            DoneSwitch.CheckedChange += (sender, e) => listener(base.AdapterPosition, DoneSwitch.Checked? 1: 0);
        }

    }
}