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
    class Repository
    {
        //Returns categories from server (now they are just generated)
        public List<Category> GetCategories()
        {
            List<Category> categories= new List<Category>();
            for (int i = 1; i <= 30; i++)
            {
                categories.Add(new Category { Name = $"Category {i}" });
            }
            return categories;
        }

        public List<Risk> GetRisks()
        {
            List<Risk> risks = new List<Risk>();
            for (int i = 0; i <= 4; i++)
            {
                risks.Add(new Risk { Name = $"Risk {i}" });
            }
            return risks;
        }
    }
}