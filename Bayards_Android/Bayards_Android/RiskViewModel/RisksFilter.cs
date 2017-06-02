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
using Java.Lang;

namespace Bayards_Android.RiskViewModel
{
    class RisksFilter : Filter
    {

        private List<Model.Risk> originalList;
        private List<Model.Risk> filteredList;
        private RisksAdapter adapter;
        public RisksFilter(RisksAdapter adapter, List<Model.Risk> originalList) : base()
        {
            this.originalList = originalList;
            filteredList = new List<Model.Risk>();
            this.adapter = adapter;

        }
        protected override FilterResults PerformFiltering(ICharSequence constraint)
        {
            filteredList.Clear();
            FilterResults results = new FilterResults();
            if (constraint == null && constraint.Length() == 0)
            {
                filteredList.AddRange(originalList);
            }
            else
            {
                filteredList = originalList
                    .Where(r => r.Name.Trim().ToLower().Contains(constraint.ToString().ToLower().Trim()))
                    .ToList();

            }

            results.Values = FromArray(filteredList.ToArray());
            results.Count = filteredList.Count;

            constraint.Dispose();
            return results;
        }

        protected override void PublishResults(ICharSequence constraint, FilterResults results)
        {
            adapter._risksList = new RisksList(filteredList);
            adapter.NotifyDataSetChanged();
        }
    }
}