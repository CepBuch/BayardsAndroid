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

namespace Bayards_Android
{
    public class RisksList
    {
        public List<Risk> Risks { get; private set; }

        public RisksList()
        {
            Repository repo = new Repository();
            Risks = repo.GetRisks();
        }

        public int NumRisks
        {
            get { return Risks.Count; }
            //+1 because of header
        }
        //Indexer
        public Risk this[int i]
        {
            get { return Risks[i]; }
            //-1 because of header
        }

    }
}