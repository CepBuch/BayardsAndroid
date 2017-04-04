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

namespace Bayards_Android
{
    [Activity(MainLauncher = true, Theme = "@style/Theme.AppCompat.Light.NoActionBar")]
    public class MainActivity : ActionBarActivity
    {
        ISharedPreferences prefs;
        RecyclerView recyclerView;
        RecyclerView.LayoutManager layoutManager;
        CategoriesList categoriesList;
        CategoriesAdapter categoriesAdapter;
        protected override void OnCreate(Bundle bundle)
        {

            base.OnCreate(bundle);
            //-------------------------------
            categoriesList = new CategoriesList();
            //--------------------------------


            //Getting info about user's authorization process from shared preferences.
            prefs = PreferenceManager.GetDefaultSharedPreferences(ApplicationContext);
            var isLanguageChosen = prefs.GetBoolean("isLanguageChosen", false);
            var isAuthorized = prefs.GetBoolean("isAuthorized", false);
            var isAcceptedAgreement = prefs.GetBoolean("isAcceptedAgreement", false);

            //If language was chosen, setting the appropriate one.
            if (isLanguageChosen)
            {
                string language_code = prefs.GetString("languageCode", "en");
                ApplyAppLanguage(language_code);
            }

            //Showing the corresponding authorizatin page.
            //If all checks have been completed, just continue. 
            if (!isLanguageChosen || !isAuthorized || !isAcceptedAgreement)
            {
                Intent intent;
                if (!isLanguageChosen)
                    intent = new Intent(this, typeof(LanguageActivity));
                else if (!isAuthorized)
                    intent = new Intent(this, typeof(PasswordActivity));
                else
                    intent = new Intent(this, typeof(AgreementActivity));

                StartActivity(intent);
                this.Finish();
            }

            //Setting up the main page
            SetContentView(Resource.Layout.MainLayout);
            //Accepting custom toolbar 
            Android.Support.V7.Widget.Toolbar toolbar =
                FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar_main);
            SetSupportActionBar(toolbar);
            //Disabling default title (as custom title with another style is shown (toolbar_main.xml)
            SupportActionBar.SetDisplayShowTitleEnabled(false);



            //-------------------------
            categoriesAdapter = new CategoriesAdapter(categoriesList);
            categoriesAdapter.ItemClick += OnItemClick;
           

            recyclerView = FindViewById<RecyclerView>(Resource.Id.recycler_view);
            recyclerView.SetAdapter(categoriesAdapter);

            layoutManager = new LinearLayoutManager(this);
            recyclerView.SetLayoutManager(layoutManager);
            //------------------------



        }

        protected void ApplyAppLanguage(string language_code)
        {
            var res = this.Resources;
            DisplayMetrics dm = res.DisplayMetrics;
            var conf = res.Configuration;
            conf.SetLocale(new Java.Util.Locale(language_code));
            res.UpdateConfiguration(conf, dm);
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
                        //Затестить и v7 и alert
                        var dialog = new Android.App.AlertDialog.Builder(this);
                        dialog.SetMessage(GetString(Resource.String.logout_message));
                        dialog.SetIcon(Resource.Drawable.en_logo);
                        //dialog.SetTitle()
                        dialog.SetPositiveButton("Yes", delegate {
                            LogOut();
                        });
                        dialog.SetNegativeButton("Cancel", delegate { });
                        dialog.Show();
                        return true;
                    }
                default:
                    return base.OnOptionsItemSelected(item);
            }
        }

        void OnItemClick(object sender, int position)
        {
            var intent = new Intent(this, typeof(RisksActivity));
            intent.PutExtra("category_id", position);
            StartActivity(intent);
        }

        public void LogOut()
        {
            //Logout process: set all steps of authorization as false
            ISharedPreferencesEditor editor = prefs.Edit();
            editor.PutBoolean("isLanguageChosen", false);
            editor.PutBoolean("isAuthorized", false);
            editor.PutBoolean("isAcceptedAgreement", false);
            editor.Apply();
            //and reload this (main) activity
            this.Recreate();
        }






    }
}

