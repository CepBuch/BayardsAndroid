using System;
using System.Collections.Generic;
using Bayards_Android.Model;

namespace Bayards_Android.RiskViewModel
{
    public class RisksList
    {
        public List<Risk> Risks { get; private set; }

        public RisksList(List<Risk> risks)
        {
            Risks = risks;
        }

        public int NumRisks
        {
            get { return Risks.Count; }
        }
        public Risk this[int i]
        {
            get { return Risks[i]; }
        }

    }
}