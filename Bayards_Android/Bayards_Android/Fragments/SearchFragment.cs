    using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Android.Support.V7.Widget;
using Bayards_Android.RiskViewModel;
using Bayards_Android.Model;
using Android.Preferences;
using Bayards_Android.SearchViewModel;

namespace Bayards_Android.Fragments
{
    public class SearchFragment : Android.Support.V4.App.Fragment
    {
        RecyclerView recyclerView;
        ISharedPreferences prefs;
        RecyclerView.LayoutManager layoutManager;
        RisksSearchAdapter risksAdapter;
        RisksList risksList;
        string language;

        public static SearchFragment newInstance()
        {
            SearchFragment fragment = new SearchFragment();
            Bundle args = new Bundle();
            fragment.Arguments = args;
            return fragment;
        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            prefs = PreferenceManager.GetDefaultSharedPreferences(Activity.ApplicationContext);

            //Getting current lunguage from application properties. 
            language = prefs.GetString("languageCode", "eng");

            InitData(language);
            base.OnCreate(savedInstanceState);
        }


        public override View OnCreateView(
            LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View view = inflater.Inflate(Resource.Layout.SearchFragment, container, false);

            recyclerView = view.FindViewById<RecyclerView>(Resource.Id.search_recycler_view);
            recyclerView.SetAdapter(risksAdapter);
            
            layoutManager = new LinearLayoutManager(Activity);
            recyclerView.SetLayoutManager(layoutManager);

            return view;
        }

        public void PerformSearch(string query)
        {
            if (View != null)
            {
                if (!string.IsNullOrWhiteSpace(query))
                {
                    var filteredData = risksList.Risks
                        .Where(r => r.Name.Trim().ToLower().Contains(query.ToLower().Trim()))
                        .ToList();
                    risksList.Risks.Clear();
                    risksList.Risks.AddRange(filteredData);
                    risksAdapter.NotifyDataSetChanged();
                }
                else
                {
                    InitData(language);
                }
            }
        }

        private void InitData(string language)
        {
            List<Risk> risks = Database.Manager.GetRisks(string.Empty, language);


            if (risks != null && risks.Count > 0)
            {
                if (risksList == null)
                {
                    risksList = new RisksList(risks);
                    risksAdapter = new RisksSearchAdapter(risksList,Context);
                    risksAdapter.ItemClick += OnItemClick;
                }
                else
                {
                    risksList.Risks.Clear();
                    risksList.Risks.AddRange(risks);
                    risksAdapter.NotifyDataSetChanged();
                }
            }
        }

        void OnItemClick(Risk clickedRisk)
        {
            //Category click event, open this category page
            var intent = new Intent(Activity, typeof(SearchRiskInfoActivity));
            intent.PutExtra("risk_id", clickedRisk.Id);
            intent.PutExtra("risk_name", clickedRisk.Name);
            StartActivity(intent);
        }
    }
}