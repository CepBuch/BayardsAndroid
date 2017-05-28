using Android.App;
using Android.Widget;
using Android.OS;
using Android.Util;
using Android.Content;
using Android.Preferences;
using Android.Support.V7.App;
using Android.Views;
using Android.Support.V7.Widget;
using SupportToolbar = Android.Support.V7.Widget.Toolbar;
using Bayards_Android.CategoryViewModel;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using Bayards_Android.Model;

using System;
using Android.Support.V4.Widget;
using Bayards_Android.Fragments;

namespace Bayards_Android
{
    [Activity(MainLauncher = true, Theme = "@style/AppTheme")]
    public class MainActivity : AppCompatActivity
    {
        ISharedPreferences prefs;
        ISharedPreferencesEditor editor;
        RecyclerView recyclerView;
        RecyclerView.LayoutManager layoutManager;
        CategoriesList categoriesList;
        CategoriesAdapter categoriesAdapter;
        private SupportToolbar toolbar;
        private ActionBarDrawerToggle drawerToggle;
        private DrawerLayout drawerLayout;
        private ListView listDrawer;
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            prefs = PreferenceManager.GetDefaultSharedPreferences(ApplicationContext);
            editor = prefs.Edit();
            
            //-----------------------PUTTING THE SERVER ADDRESS-------------------------------
            editor.PutString("hosting_address", "http://vhost29450.cpsite.ru");
            editor.Apply();

            //Check user's authorization stage
            bool passedAllChecks = CheckStepsOfAuthorization();

            if (passedAllChecks)
            {
                SetContentView(Resource.Layout.MainActivity);

                //Accepting toolbar and drawerlayout
                toolbar = FindViewById<SupportToolbar>(Resource.Id.toolbar_main);
                drawerLayout = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);
                listDrawer = FindViewById<ListView>(Resource.Id.drawer_list);
                SetSupportActionBar(toolbar);


                drawerToggle = new ActionBarDrawerToggle(this, drawerLayout,
                    Resource.String.openDrawer, Resource.String.closeDrawer);

                drawerLayout.AddDrawerListener(drawerToggle);
                SupportActionBar.SetHomeButtonEnabled(true);
                SupportActionBar.SetDisplayHomeAsUpEnabled(true);
                SupportActionBar.SetDisplayShowTitleEnabled(false);
                drawerToggle.SyncState();


                //ShowAllCategories();
                var trans = SupportFragmentManager.BeginTransaction();
                trans.Add(Resource.Id.mainFragmentContainer,
                    CategoriesContainerFragment.newInstance(string.Empty), "CategoriesContainerFragment");
                trans.Commit();
            }
        }


        private void ShowAllCategories()
        {
            //Getting current lunguage from application properties. 
            string language = prefs.GetString("languageCode", "eng");

            var categories = Database.Manager.GetCategories(language);

            if (categories != null && categories.Count > 0)
            {
                categoriesList = new CategoriesList(categories);
                categoriesAdapter = new CategoriesAdapter(categoriesList);
                categoriesAdapter.ItemClick += OnItemClick;

                //recyclerView = FindViewById<RecyclerView>(Resource.Id.recycler_view);
                ////recyclerView.SetAdapter(categoriesAdapter);

                //layoutManager = new LinearLayoutManager(this);
                //recyclerView.SetLayoutManager(layoutManager);
            }
            else
            {
                var dialog = new Android.App.AlertDialog.Builder(this);
                string message = GetString(Resource.String.data_reading_error);
                dialog.SetMessage(message);
                dialog.SetPositiveButton("Ok", delegate
                {
                    editor.PutBoolean("isDataLoaded", false);
                    editor.Apply();
                    CheckStepsOfAuthorization();
                });
                dialog.Show();
            }
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.menu_main, menu);
            return base.OnPrepareOptionsMenu(menu);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            drawerToggle.OnOptionsItemSelected(item);

            //switch (item.ItemId)
            //{
            //    //case Resource.Id.menu_logout:
            //    //    {
            //    //        var dialog = new Android.App.AlertDialog.Builder(this);
            //    //        dialog.SetMessage(GetString(Resource.String.logout_message));

            //    //        dialog.SetPositiveButton("Yes", (s, e) => LogOut());
            //    //        dialog.SetNegativeButton("Cancel", delegate { });
            //    //        dialog.Show();
            //    //        return true;
            //    //    }
            //    //case Resource.Id.menu_settings:
            //    //    {
            //    //        return true;
            //    //    }
            //    default:
            return base.OnOptionsItemSelected(item);
        //}
    }

    void OnItemClick(object sender, Category clicked_category)
    {
        //Category click event, open this category page
        var intent = new Intent(this, typeof(RisksActivity));
        intent.PutExtra("category_id", clicked_category.Id);
        intent.PutExtra("category_name", clicked_category.Name);
        StartActivity(intent);
    }

    public bool CheckStepsOfAuthorization()
    {
        //Getting info about user's authorization process from shared preferences.
        var isLanguageChosen = prefs.GetBoolean("isLanguageChosen", false);
        var isAuthorized = prefs.GetBoolean("isAuthorized", false);
        var isAcceptedAgreement = prefs.GetBoolean("isAcceptedAgreement", false);
        var isDataLoaded = prefs.GetBoolean("isDataLoaded", false);

        //If language was chosen, setting the appropriate one.
        if (isLanguageChosen)
        {
            string language_code = prefs.GetString("languageCode", "eng");
            ApplyAppLanguage(language_code);
        }

        //Showing the corresponding authorizatin page.
        //If all checks have been completed, just continue. 
        if (!isLanguageChosen || !isAuthorized || !isAcceptedAgreement || !isDataLoaded)
        {
            Intent intent;
            if (!isLanguageChosen)
                intent = new Intent(this, typeof(LanguageActivity));
            else if (!isAuthorized)
                intent = new Intent(this, typeof(PasswordActivity));
            else if (!isAcceptedAgreement)
                intent = new Intent(this, typeof(AgreementActivity));
            else
                intent = new Intent(this, typeof(DataLoadActivity));

            StartActivity(intent);
            this.Finish();
            return false;
        }

        return true;
    }

    protected void ApplyAppLanguage(string language_code)
    {
        var res = this.Resources;
        DisplayMetrics dm = res.DisplayMetrics;
        var conf = res.Configuration;
        conf.SetLocale(new Java.Util.Locale(language_code));
        res.UpdateConfiguration(conf, dm);
    }

    public void LogOut()
    {
        //Logout process: set all steps of authorization as false
        editor.PutBoolean("isLanguageChosen", false);
        editor.PutBoolean("isAuthorized", false);
        editor.PutBoolean("isAcceptedAgreement", false);
        editor.PutBoolean("isDataLoaded", false);
        editor.Apply();

        //and reload this (main) activity
        this.Recreate();
    }
}
}

