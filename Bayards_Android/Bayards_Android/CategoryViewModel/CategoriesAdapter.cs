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
    class CategoriesAdapter : RecyclerView.Adapter
    {

        public event EventHandler<int> ItemClick;

        public CategoriesList _categoriesList;

        private const int TYPE_HEADER = 0;
        private const int TYPE_ITEM = 1;
        public CategoriesAdapter(CategoriesList categoriesList)
        {
            _categoriesList = categoriesList;
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            //Accepting base view of category
            //Also checking wether new item in List is header or general item. 
            RecyclerView.ViewHolder ch;
            View itemView;
            if (viewType == TYPE_ITEM)
            {
                itemView = LayoutInflater.From(parent.Context).
                           Inflate(Resource.Layout.CategoryView, parent, false);
                ch = new CategoryViewHolder(itemView, OnClick);
            }
            else
            {
                itemView = LayoutInflater.From(parent.Context).
                           Inflate(Resource.Layout.HeaderView, parent, false);
                ch = new HeaderViewHolder(itemView);

            }
            return ch;
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            //Modifying base view with exact data
            if (holder is CategoryViewHolder)
            {
                CategoryViewHolder ch = holder as CategoryViewHolder;
                ch.Button.Text = _categoriesList[position].Name;
            }
        }

        private bool IsPoitionHeader(int position)
            => position == 0;

        public override int GetItemViewType(int position)
        {
            if (IsPoitionHeader(position))
                return TYPE_HEADER;
            else 
                return TYPE_ITEM;
 
        }

        public override int ItemCount
        {
            get { return _categoriesList.NumCategories; }
        }

        void OnClick(int position)
        {
            ItemClick?.Invoke(this, position);
        }



    }
}