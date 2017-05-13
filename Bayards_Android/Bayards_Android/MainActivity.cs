using Android.App;
using Android.Widget;
using Android.OS;
using Android.Util;
using Android.Content;
using Android.Preferences;
using Android.Support.V7.App;
using Android.Views;
using Android.Support.V7.Widget;
using Bayards_Android.CategoryViewModel;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using Bayards_Android.Model;
using System;

namespace Bayards_Android
{
    [Activity(MainLauncher = true, Theme = "@style/Theme.AppCompat.Light.NoActionBar")]
    public class MainActivity : ActionBarActivity
    {
        ISharedPreferences prefs;
        ISharedPreferencesEditor editor;
        RecyclerView recyclerView;
        RecyclerView.LayoutManager layoutManager;
        CategoriesList categoriesList;
        CategoriesAdapter categoriesAdapter;
        protected override void OnCreate(Bundle bundle)
        {

            base.OnCreate(bundle);

            prefs = PreferenceManager.GetDefaultSharedPreferences(ApplicationContext);
            editor = prefs.Edit();

            //Chec user's authorization stage
            bool passedAllChecks = CheckStepsOfAuthorization();

            if (passedAllChecks)
            {
                SetContentView(Resource.Layout.MainLayout);

                //Accepting custom toolbar 
                Android.Support.V7.Widget.Toolbar toolbar =
                    FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar_main);
                SetSupportActionBar(toolbar);
                SupportActionBar.SetDisplayShowTitleEnabled(false);

                ShowAllCategories();
            }
        }


        private void ShowAllCategories()
        {

            var categories = Database.Manager.Categories;

            if (categories != null && categories.ToList().Count > 0)
            {
                categoriesList = new CategoriesList(categories.ToList());
                categoriesAdapter = new CategoriesAdapter(categoriesList);
                categoriesAdapter.ItemClick += OnItemClick;

                recyclerView = FindViewById<RecyclerView>(Resource.Id.recycler_view);
                recyclerView.SetAdapter(categoriesAdapter);

                layoutManager = new LinearLayoutManager(this);
                recyclerView.SetLayoutManager(layoutManager);
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
            switch (item.ItemId)
            {
                case Resource.Id.menu_logout:
                    {
                        var dialog = new Android.App.AlertDialog.Builder(this);
                        dialog.SetMessage(GetString(Resource.String.logout_message));

                        dialog.SetPositiveButton("Yes", (s, e) => LogOut());
                        dialog.SetNegativeButton("Cancel", delegate { });
                        dialog.Show();
                        return true;
                    }
                case Resource.Id.menu_settings:
                    {
                        return true;
                    }
                default:
                    return base.OnOptionsItemSelected(item);
            }
        }

        void OnItemClick(object sender, int position)
        {
            //Category click event, open this category page
            var intent = new Intent(this, typeof(RisksActivity));
            intent.PutExtra("category_id", position);
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

