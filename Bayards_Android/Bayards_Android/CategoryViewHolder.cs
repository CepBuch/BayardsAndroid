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

namespace Bayards_Android
{
    class CategoryViewHolder: RecyclerView.ViewHolder
    {
        public Button Button { get; private set; }

        //Defining view of each RecycleView item
        public CategoryViewHolder(View itemView):base(itemView)
        {
            Button = itemView.FindViewById<Button>(Resource.Id.categoryButton);
        }
    }
}