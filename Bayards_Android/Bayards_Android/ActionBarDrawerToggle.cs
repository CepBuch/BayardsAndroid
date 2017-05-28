using Android.Support.V4.Widget;
using Android.Support.V7.App;
using System;
using SupportActionBarDrawerToggle = Android.Support.V7.App.ActionBarDrawerToggle;
using Android.Views;

namespace Bayards_Android
{
    class ActionBarDrawerToggle : SupportActionBarDrawerToggle
    {
        private AppCompatActivity HostActivity;
        private int OpenedResource;
        private int ClosedResource; 
        public ActionBarDrawerToggle(AppCompatActivity host, DrawerLayout drawerLayout,
            int openedResource, int closedResource) : base(host, drawerLayout, openedResource, closedResource)
        {
            HostActivity = host;
            OpenedResource = openedResource;
            ClosedResource = closedResource;
        }

        public override void OnDrawerOpened(View drawerView)
        {
            base.OnDrawerOpened(drawerView);
        }

        public override void OnDrawerClosed(View drawerView)
        {
            base.OnDrawerClosed(drawerView);
        }

        public override void OnDrawerSlide(View drawerView, float slideOffset)
        {
            base.OnDrawerSlide(drawerView, slideOffset);
        }
    }
}