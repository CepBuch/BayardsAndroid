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
using Bayards_Android.RiskViewModel;
using Bayards_Android.CategoryViewModel;

namespace Bayards_Android.SearchViewModel
{
    class RisksSearchAdapter : RecyclerView.Adapter
    {
        public RisksList _risksList;
        Context context;
        private const int TYPE_HEADER = 0;
        private const int TYPE_ITEM = 1;


        public event Action<Model.Risk> ItemClick;
        public RisksSearchAdapter(RisksList risksList, Context context)
        {
            _risksList = risksList;
            this.context = context;
        }
        public override int ItemCount
        {
            get { return _risksList.NumRisks + 1; }
        }


        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            //Accepting base view of item
            //Also checking wether new item in List is header or general item. 
            RecyclerView.ViewHolder rh;
            View itemView;


            if (viewType == TYPE_ITEM)
            {
                itemView = LayoutInflater.From(parent.Context).
                           Inflate(Resource.Layout.RisksSearchView, parent, false);
                rh = new SearchRiskViewHolder(itemView, OnClick);
            }
            else
            {
                itemView = LayoutInflater.From(parent.Context).
                           Inflate(Resource.Layout.HeaderView, parent, false);
                rh = new SearchHeaderViewHolder(itemView);

            }
            return rh;
        }
        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            //Modifying base view with exact data
            if (holder is SearchRiskViewHolder)
            {
                SearchRiskViewHolder rh = holder as SearchRiskViewHolder;
                rh.ContentButton.Text = _risksList[position - 1].Name;
            }
            else if(holder is SearchHeaderViewHolder)
            {
                SearchHeaderViewHolder hh = holder as SearchHeaderViewHolder;
                hh.TextView.Text = context.GetString(Resource.String.found_risks);
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


        void OnClick(int position)
        {
            ItemClick?.Invoke(_risksList[position - 1]);
        }
    }
}