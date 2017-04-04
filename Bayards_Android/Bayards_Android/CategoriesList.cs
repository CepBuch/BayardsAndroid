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
    class CategoriesList
    {
        public List<string>  Categories { get; set; }


        //Counter
        public int NumCategories
        {
            get { return Categories.Count + 1;  }
            //+1 because of header
        }


        //Indexer
        public string this[int i]
        {
            get { return Categories[i-1]; }
            //-1 because of header
        }
        public CategoriesList()
        {
            //Getting all available categories
            Categories = GetCategories();
          
        }

        //Returns categories from server (now they are just generated)
        public List<string> GetCategories()
        {
            List<string> categoriesToFill = new List<string>();
            for (int i = 1; i <= 30; i++)
            {
                categoriesToFill.Add($"Category {i}");
            }
            return categoriesToFill;
        }





    }
}