using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Android.Preferences;
using Bayards_Android.Model;
using Android.Support.V7.Widget;
using Bayards_Android.CategoryViewModel;

namespace Bayards_Android.Fragments
{
    public class CategoriesContainerFragment : Android.Support.V4.App.Fragment
    {
        private static string CATEGORY_ID = "category_id";
        ISharedPreferences prefs;
        RecyclerView recyclerView;
        RecyclerView.LayoutManager layoutManager;
        CategoriesList categoriesList;
        CategoriesAdapter categoriesAdapter;

        public static CategoriesContainerFragment newInstance(string category_id)
        {
            CategoriesContainerFragment fragment = new CategoriesContainerFragment();
            Bundle args = new Bundle();
            args.PutString(CATEGORY_ID, category_id);
            fragment.Arguments = args;
            return fragment;
        }

        public override void OnCreate(Bundle savedInstanceState)
        {

            prefs = PreferenceManager.GetDefaultSharedPreferences(Activity.ApplicationContext);

            //Getting current lunguage from application properties. 
            string language = prefs.GetString("languageCode", "eng");
            string category_id = Arguments.GetString(CATEGORY_ID, string.Empty);

            InitData(category_id, language);
            base.OnCreate(savedInstanceState);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View view = inflater.Inflate(Resource.Layout.CategoriesFragment, container, false);

            recyclerView = view.FindViewById<RecyclerView>(Resource.Id.recycler_view);
            recyclerView.SetAdapter(categoriesAdapter);

            layoutManager = new LinearLayoutManager(Activity);
            recyclerView.SetLayoutManager(layoutManager);

            return view;
        }

        void OnItemClick(object sender, Category clicked_category)
        {
            //Category click event, open this category page
            var intent = new Intent(Activity, typeof(CategoryContentActivity));
            intent.PutExtra("category_id", clicked_category.Id);
            intent.PutExtra("category_name", clicked_category.Name);
            StartActivity(intent);
        }



        private void InitData(string category_id, string language)
        {
            List<Category> categories = new List<Category>();

            if (string.IsNullOrEmpty(category_id))
            {
                categories = Database.Manager.GetCategories(language);
            }
            else
            {
                categories = Database.Manager.GetSubcategories(category_id, language);
            }


            if (categories != null && categories.Count > 0)
            {
                categoriesList = new CategoriesList(categories);
                categoriesAdapter = new CategoriesAdapter(categoriesList);
                categoriesAdapter.ItemClick += OnItemClick;
            }
        }
    }
}