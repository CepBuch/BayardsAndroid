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
    class CategoryContentPagerAdapter: FragmentPagerAdapter
    {
        string category_id;

        public CategoryContentPagerAdapter(Android.Support.V4.App.FragmentManager fm, string category_id)
            : base(fm)
        {
            this.category_id = category_id;
        }

        public override Android.Support.V4.App.Fragment GetItem(int position)
        {
            switch (position)
            {
                case 0:
                    return (Android.Support.V4.App.Fragment)RisksContainerFragment.newInstance(category_id);
                case 1:
                    return (Android.Support.V4.App.Fragment)CategoriesContainerFragment.newInstance(category_id);
                default:
                    return (Android.Support.V4.App.Fragment)RisksContainerFragment.newInstance(category_id);
            }
            
        }

        public override ICharSequence GetPageTitleFormatted(int position)
        {
            switch (position)
            {
                case 0:
                    return new Java.Lang.String("Risks");
                case 1:
                    return new Java.Lang.String("Tasks");
                default:
                    return new Java.Lang.String("Risks");
            }
        }

        public override int Count
        {
            get
            {
                return 2;
            }
        }

    }
}