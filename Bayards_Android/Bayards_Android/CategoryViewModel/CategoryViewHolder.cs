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

namespace Bayards_Android.CategoryViewModel
{
    class CategoryViewHolder: RecyclerView.ViewHolder
    {
        public Button ContentButton { get; private set; }

        //Defining view of each RecycleView item
        public CategoryViewHolder(View itemView, Action<int> listener)
            :base(itemView)
        {
            ContentButton = itemView.FindViewById<Button>(Resource.Id.categoryButton);
            ContentButton.Selected = true;
            ContentButton.Click += (sender, e) => listener(base.AdapterPosition);

        }
    }

    class HeaderViewHolder : RecyclerView.ViewHolder
    {
        public TextView TextView { get; private set; }

        public HeaderViewHolder(View itemView):base(itemView)
        {
            TextView = itemView.FindViewById<TextView>(Resource.Id.headerTextView);
        }
    }
}