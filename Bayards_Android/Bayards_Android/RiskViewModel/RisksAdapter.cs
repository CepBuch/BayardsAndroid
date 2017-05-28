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
    class RisksAdapter : RecyclerView.Adapter
    {
        public RisksList _risksList;

        public RisksAdapter(RisksList risksList)
        {
            _risksList = risksList;
        }
        public override int ItemCount
        {

            get { return _risksList.NumRisks; }
        }


        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            RecyclerView.ViewHolder rh;
            View itemView = LayoutInflater.From(parent.Context).
                           Inflate(Resource.Layout.RiskView, parent, false);
            rh = new RiskViewHolder(itemView);
            return rh;
        }
        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            //Modifying base view with exact data
            if (holder is RiskViewHolder)
            {
                RiskViewHolder rh = holder as RiskViewHolder;
                //ch.ContentButton.Text = _categoriesList[position - 1].Name;
            }
        }
    }
}