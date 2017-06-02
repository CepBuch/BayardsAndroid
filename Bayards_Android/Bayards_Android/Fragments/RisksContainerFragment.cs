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
using Bayards_Android.Model;
using Bayards_Android.RiskViewModel;
using Android.Support.V7.Widget;
using Android.Preferences;

namespace Bayards_Android.Fragments
{
    class RisksContainerFragment : Android.Support.V4.App.Fragment
    {

        private static string CATEGORY_ID = "category_id";
        RecyclerView recyclerView;
        ISharedPreferences prefs;
        RecyclerView.LayoutManager layoutManager;
        RisksAdapter risksAdapter;
        RisksList risksList;


        public static RisksContainerFragment newInstance(string category_id)
        {
            RisksContainerFragment fragment = new RisksContainerFragment();
            Bundle args = new Bundle();
            args.PutString(CATEGORY_ID, category_id);

            fragment.Arguments = args;
            return fragment;
        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            prefs = PreferenceManager.GetDefaultSharedPreferences(Activity.ApplicationContext);

            //Getting current lunguage from application properties. 
            string language = prefs.GetString("languageCode", "eng");
            string category_id = Arguments.GetString(CATEGORY_ID, string.Empty);

            InitData(category_id, language);
            base.OnCreate(savedInstanceState);
        }


        public override View OnCreateView(
            LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View view = inflater.Inflate(Resource.Layout.RisksFragment, container, false);

            recyclerView = view.FindViewById<RecyclerView>(Resource.Id.recycler_view);
            recyclerView.SetAdapter(risksAdapter);

            layoutManager = new LinearLayoutManager(Activity);
            recyclerView.SetLayoutManager(layoutManager);

            return view;
        }

        private void InitData(string category_id, string language)
        {
            List<Risk> risks = new List<Risk>();
            risks = Database.Manager.GetRisks(category_id, language);

            if (risks != null && risks.Count > 0)
            {
                risksList = new RisksList(risks);
                risksAdapter = new RisksAdapter(Context,risksList);
                risksAdapter.ItemClick += OnItemClick;
            }
        }
        void OnItemClick(Risk clickedRisk, int isChecked)
        {
            Database.Manager.CheckRiskAsViewed(clickedRisk.Id, isChecked);
        }
    }
}