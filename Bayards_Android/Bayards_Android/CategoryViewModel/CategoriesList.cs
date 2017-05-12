using System;
using System.Collections.Generic;
using Bayards_Android.Model;

namespace Bayards_Android.CategoryViewModel
{
    class CategoriesList
    {

        public List<Category> Categories { get; set; }

        public CategoriesList(List<Category> categories)
        {
            Categories = categories;
        }

        //Counter
        public int NumCategories
        {
            get { return Categories.Count; }
        }
        //Indexer
        public Category this[int i]
        {
            get { return Categories[i]; }
        }
    }
}