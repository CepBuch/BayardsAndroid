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

namespace Bayards_Android
{
    class RiskInfoFragment: Android.Support.V4.App.Fragment
    {

        private static string RISK_NAME = "risk_name";
        public RiskInfoFragment() {  }

        public static RiskInfoFragment newInstance(string name)
        {
            RiskInfoFragment fragment = new RiskInfoFragment();
            Bundle args = new Bundle();
            args.PutString(RISK_NAME, name);
            fragment.Arguments = args;
            return fragment;
        }
        public override View OnCreateView(
            LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            string name = Arguments.GetString(RISK_NAME, "");
            View view = inflater.Inflate(Resource.Layout.RiskInfoLayout, container, false);
            TextView riskName = view.FindViewById<TextView>(Resource.Id.risk_name);
            riskName.Text = name;
            return view;
        }
    }
}