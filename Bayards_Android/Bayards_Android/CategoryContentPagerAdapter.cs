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
using Android.Support.V4.View;
using Android.Support.V4.App;
using Bayards_Android.Fragments;
using Java.Lang;

namespace Bayards_Android
{
    class CategoryContentPagerAdapter : FragmentStatePagerAdapter
    {
        string category_id;
        List<Android.Support.V4.App.Fragment> fragments;
        List<string> titles;

        public CategoryContentPagerAdapter(Android.Support.V4.App.FragmentManager fm, string category_id)
            : base(fm)
        {
            this.category_id = category_id;

            fragments = new List<Android.Support.V4.App.Fragment>
            {
                RisksContainerFragment.newInstance(category_id) ,
                CategoriesContainerFragment.newInstance(category_id)
            };
            titles = new List<string>
            {
                "Risks",
                "Tasks"
            };
        }


        public override Android.Support.V4.App.Fragment GetItem(int position)
        {
            return fragments[position];
        }

        public override ICharSequence GetPageTitleFormatted(int position)
        {
            return new Java.Lang.String(titles[position]);
        }



        public override int Count
        {
            get
            {
                return fragments.Count;
            }
        }

        public void RemovePage(string tabName)
        {
            var index = titles.FindIndex(t => t.ToLower() == tabName.ToLower());
            if (index != -1)
            {
                fragments.RemoveAt(index);
                titles.RemoveAt(index);
                NotifyDataSetChanged();
            }
        }


    }
}