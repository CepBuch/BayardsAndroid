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

namespace Bayards_Android.RiskViewModel
{
    class RiskContentFragment : Android.Support.V4.App.Fragment
    {

        private static string RISK_NAME = "risk_name";
        private static string RISK_CONTENT = "risk_content";
        private static string RISK_IMAGES = "risk_images";
        private static string RISK_VIDEOS = "risk_videos";
        public RiskContentFragment() { }

        public static RiskContentFragment newInstance(Risk risk)
        {
            RiskContentFragment fragment = new RiskContentFragment();
            Bundle args = new Bundle();
            args.PutString(RISK_NAME, risk.Name);
            args.PutString(RISK_CONTENT, risk.Content);

            var mediaForRisk = Database.Manager.GetMedia(risk.Id, risk.Language);

            if (mediaForRisk != null)
            {
                args.PutStringArray(RISK_IMAGES, mediaForRisk
                    .Where(m => m != null && !string.IsNullOrWhiteSpace(m.Name) && m.TypeMedia == Enums.TypeMedia.Image)
                    .Select(m => m.Name).ToArray());

                args.PutStringArray(RISK_VIDEOS, mediaForRisk
                    .Where(m => m != null && !string.IsNullOrWhiteSpace(m.Name) && m.TypeMedia == Enums.TypeMedia.Video)
                    .Select(m => m.Name).ToArray());

            }

            fragment.Arguments = args;
            return fragment;
        }



        public override View OnCreateView(
            LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            string name = Arguments.GetString(RISK_NAME, "");
            string content = Arguments.GetString(RISK_CONTENT, "");
            var imagePaths = Arguments.GetStringArray(RISK_IMAGES);
            var videoIds = Arguments.GetStringArray(RISK_VIDEOS);
            View view = inflater.Inflate(Resource.Layout.RiskInfoLayout, container, false);
            TextView riskNameView = view.FindViewById<TextView>(Resource.Id.risk_name);
            TextView contentView = view.FindViewById<TextView>(Resource.Id.risk_content);

            riskNameView.Text = name;
            contentView.Text = content;


            //---------

            ExpandableListView expandableListView = view.FindViewById<ExpandableListView>(Resource.Id.expandableListView);

            bool hasImages = imagePaths != null && imagePaths.Length > 0;
            bool hasVideos = videoIds != null && videoIds.Length > 0;



            Dictionary<string, List<string>> dicParams = new Dictionary<string, List<string>>();
            List<string> groupNames = new List<string>();
            

            if (hasImages)
            {
                List<string> images = imagePaths.ToList();
                groupNames.Add("Images");
                dicParams.Add("Images", images);
            }

            if (hasVideos)

            {
                List<string> videos = videoIds.ToList();
                groupNames.Add("Videos");
                dicParams.Add("Videos", videos);
            }

            ExpandableListViewAdapter mAdapter = new ExpandableListViewAdapter(Activity, groupNames, dicParams);
            expandableListView.SetAdapter(mAdapter);
            return view;
        }





    }
}