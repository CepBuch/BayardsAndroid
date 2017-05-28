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

        //private static string RISK_NAME = "risk_name";
        //private static string RISK_CONTENT = "risk_content";
        //private static string RISK_IMAGES = "risk_images";
        //private static string RISK_VIDEOS = "risk_videos";


        RecyclerView recyclerView;
        ISharedPreferences prefs;
        RecyclerView.LayoutManager layoutManager;
        RisksAdapter risksAdapter;
        RisksList risksList;

        public RisksContainerFragment() { }

        public static RisksContainerFragment newInstance(string category_id)
        {
            RisksContainerFragment fragment = new RisksContainerFragment();
            Bundle args = new Bundle();
            args.PutString(CATEGORY_ID, category_id);

            fragment.Arguments = args;
            return fragment;
            //args.PutString(RISK_NAME, risk.Name);
            //args.PutString(RISK_CONTENT, risk.Content);

            //var mediaForRisk = Database.Manager.GetMedia(risk.Id, risk.Language);

            //if (mediaForRisk != null)
            //{
            //    args.PutStringArray(RISK_IMAGES, mediaForRisk
            //        .Where(m => m != null && !string.IsNullOrWhiteSpace(m.Name) && m.TypeMedia == Enums.TypeMedia.Image)
            //        .Select(m => m.Name).ToArray());

            //    args.PutStringArray(RISK_VIDEOS, mediaForRisk
            //        .Where(m => m != null && !string.IsNullOrWhiteSpace(m.Name) && m.TypeMedia == Enums.TypeMedia.Video)
            //        .Select(m => m.Name).ToArray());

            //}


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

            //    string name = Arguments.GetString(RISK_NAME, "");
            //    string content = Arguments.GetString(RISK_CONTENT, "");
            //    var imagePaths = Arguments.GetStringArray(RISK_IMAGES);
            //    var videoIds = Arguments.GetStringArray(RISK_VIDEOS);


            //    TextView riskNameView = view.FindViewById<TextView>(Resource.Id.risk_name);
            //    TextView contentView = view.FindViewById<TextView>(Resource.Id.risk_content);

            //    riskNameView.Text = name;
            //    contentView.Text = content;


            //    //---------

            //    ExpandableListView expandableListView = view.FindViewById<ExpandableListView>(Resource.Id.expandableListView);

            //    bool hasImages = imagePaths != null && imagePaths.Length > 0;
            //    bool hasVideos = videoIds != null && videoIds.Length > 0;



            //    Dictionary<string, List<string>> dicParams = new Dictionary<string, List<string>>();
            //    List<string> groupNames = new List<string>();


            //    if (hasImages)
            //    {
            //        List<string> images = imagePaths.ToList();
            //        groupNames.Add("Images");
            //        dicParams.Add("Images", images);
            //    }

            //    if (hasVideos)

            //    {
            //        List<string> videos = videoIds.ToList();
            //        groupNames.Add("Videos");
            //        dicParams.Add("Videos", videos);
            //    }

            //    ExpandableListViewAdapter mAdapter = new ExpandableListViewAdapter(Activity, groupNames, dicParams);
            //    expandableListView.SetAdapter(mAdapter);
            //}
        }

        private void InitData(string category_id, string language)
        {
            List<Risk> risks = new List<Risk>();
            risks = Database.Manager.GetRisks(category_id, language);

            if (risks != null && risks.Count > 0)
            {
                risksList = new RisksList(risks);
                risksAdapter = new RisksAdapter(risksList);
            }
        }
    }
}