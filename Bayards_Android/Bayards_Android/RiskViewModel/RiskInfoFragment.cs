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

namespace Bayards_Android.RiskViewModel
{
    class RiskInfoFragment : Android.Support.V4.App.Fragment
    {

        private static string RISK_NAME = "risk_name";
        private static string IMAGE_ID = "image_id";
        private static string CONTENT_ID = "content_id";
        public RiskInfoFragment() { }

        public static RiskInfoFragment newInstance(string name, int image, int content_id)
        {
            RiskInfoFragment fragment = new RiskInfoFragment();
            Bundle args = new Bundle();
            args.PutString(RISK_NAME, name);
            args.PutInt(IMAGE_ID, image);
            args.PutInt(CONTENT_ID, content_id);
            fragment.Arguments = args;
            return fragment;
        }
        public override View OnCreateView(
            LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            string name = Arguments.GetString(RISK_NAME, "");
            int content_id = Arguments.GetInt(CONTENT_ID, -1);
            int image_id = Arguments.GetInt(IMAGE_ID, -1);

            View view = inflater.Inflate(Resource.Layout.RiskInfoLayout, container, false);
            TextView riskNameView = view.FindViewById<TextView>(Resource.Id.risk_name);
            ImageView imageView = view.FindViewById<ImageView>(Resource.Id.risk_picture);
            TextView contentView = view.FindViewById<TextView>(Resource.Id.risk_content);

            riskNameView.Text = name;
            imageView.SetImageResource(image_id);
            contentView.Text = Resources.GetString(content_id);

            return view;
        }
    }
}