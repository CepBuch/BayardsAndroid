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
    class CategoriesAdapter : RecyclerView.Adapter
    {
        public CategoriesList _categoriesList;
        public CategoriesAdapter(CategoriesList categoriesList)
        {
            _categoriesList = categoriesList;
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            //Accepting base view of category
            View itemView = LayoutInflater.From(parent.Context).
                        Inflate(Resource.Layout.CategoryView, parent, false);

            CategoryViewHolder ch = new CategoryViewHolder(itemView);
            return ch;
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            //Modifying base view with exact data
            CategoryViewHolder ch = holder as CategoryViewHolder;
            ch.Button.Text = _categoriesList[position];
        }

        public override int ItemCount
        {
            get { return _categoriesList.NumCategories; }
        }



    }
}