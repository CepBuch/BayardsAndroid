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
    class CategoriesList
    {
        public List<Category> Categories { get; set; }

        public CategoriesList()
        {
            //Getting all available categories
            Repository repo = new Repository();
            Categories = repo.GetCategories();

        }

        //Counter
        public int NumCategories
        {
            get { return Categories.Count + 1; }
            //+1 because of header
        }
        //Indexer
        public Category this[int i]
        {
            get { return Categories[i - 1]; }
            //-1 because of header
        }
    }
}