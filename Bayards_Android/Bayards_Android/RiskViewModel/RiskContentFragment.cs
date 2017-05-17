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
    class RiskContentFragment : Android.Support.V4.App.Fragment
    {

        private static string RISK_NAME = "risk_name";
        private static string RISK_CONTENT = "risk_content";
        public RiskContentFragment() { }

        public static RiskContentFragment newInstance(string name, string content)
        {
            RiskContentFragment fragment = new RiskContentFragment();
            Bundle args = new Bundle();
            args.PutString(RISK_NAME, name);
            args.PutString(RISK_CONTENT, content);
            fragment.Arguments = args;
            return fragment;
        }



        public override View OnCreateView(
            LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            string name = Arguments.GetString(RISK_NAME, "");
            string content = Arguments.GetString(RISK_CONTENT, "");

            View view = inflater.Inflate(Resource.Layout.RiskInfoLayout, container, false);
            TextView riskNameView = view.FindViewById<TextView>(Resource.Id.risk_name);
            TextView contentView = view.FindViewById<TextView>(Resource.Id.risk_content);

            riskNameView.Text = name;
            contentView.Text = content;

            return view;
        }


    }
}