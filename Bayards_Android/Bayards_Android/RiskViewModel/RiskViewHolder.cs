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
        public Switch DoneSwitch;

        public TextView NameTextView { get; set; }
        public TextView ContentTextView { get; set; }
        public TextView DoneInfoTextView { get; set; }

        //Defining view of each RecycleView item
        public RiskViewHolder(View itemView, Action<int, int> listener)
            : base(itemView)
        {
            NameTextView = itemView.FindViewById<TextView>(Resource.Id.risk_name);
            ContentTextView = itemView.FindViewById<TextView>(Resource.Id.risk_content);
            DoneSwitch = itemView.FindViewById<Switch>(Resource.Id.done_switch);
            DoneInfoTextView = itemView.FindViewById<TextView>(Resource.Id.done_info);
            UpdateTextViews();
            DoneSwitch.CheckedChange += (sender, e) =>
            {
                listener(base.AdapterPosition, DoneSwitch.Checked ? 1 : 0);
                UpdateTextViews();
            };
        }

        private void UpdateTextViews()
        {

            DoneInfoTextView.Text = ItemView.Context.GetString(DoneSwitch.Checked ? Resource.String.done : Resource.String.not_done);
        }

    }
}