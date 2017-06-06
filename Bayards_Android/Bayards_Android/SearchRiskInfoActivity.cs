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
using SupportToolbar = Android.Support.V7.Widget.Toolbar;
using Android.Preferences;
using Android.Support.V7.App;
using Bayards_Android.Model;
using Com.Bumptech.Glide;
using Com.Bumptech.Glide.Load.Engine;
using Android.Graphics;
using Android.Text.Method;

namespace Bayards_Android
{
    [Activity(Theme = "@style/AppTheme")]
    public class SearchRiskInfoActivity : AppCompatActivity
    {
        SupportToolbar toolbar;
        ISharedPreferences prefs;
        string language;


        public List<MediaObject> images;
        public List<MediaObject> videos;

        public Switch DoneSwitch;

        public TextView NameTextView { get; set; }
        public TextView ContentTextView { get; set; }
        public TextView DoneInfoTextView { get; set; }

        public RelativeLayout HeaderImagesLayout { get; set; }
        public LinearLayout ExpandableImagesLayout { get; set; }

        public ImageView ImageHeader { get; set; }

        public RelativeLayout HeaderVideosLayout { get; set; }
        public LinearLayout ExpandableVideosLayout { get; set; }

        public ImageView VideoHeader { get; set; }



        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            prefs = PreferenceManager.GetDefaultSharedPreferences(ApplicationContext);
            language = prefs.GetString("languageCode", "eng");

            SetContentView(Resource.Layout.SearchRiskInfoActivity);


            //Enabling custom toolbar
            toolbar = FindViewById<SupportToolbar>(Resource.Id.toolbar_risks);
            TextView toolbarTitle = FindViewById<TextView>(Resource.Id.toolbar_title);
            SetSupportActionBar(toolbar);

            //Disabling default title;
            SupportActionBar.SetDisplayShowTitleEnabled(false);

            //Showing name of parent_category as toolbar title
            var parent_category_name = Intent.GetStringExtra("risk_name");
            if (!string.IsNullOrWhiteSpace(parent_category_name))
                toolbarTitle.Text = parent_category_name;

            //Enabling BackButton
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            SupportActionBar.SetDisplayShowHomeEnabled(true);



            //Getting Category id from previous activity.
            var risk_id = Intent.GetStringExtra("risk_id");
            var found_risk = Database.Manager.FindRisk(risk_id, language);



            if(found_risk != null)
            {
                FillInfoAboutRisk(found_risk);
            }
            else
            {
                Finish();
            }

        }

        private void FillInfoAboutRisk(Model.Risk risk)
        {
            NameTextView = FindViewById<TextView>(Resource.Id.risk_name);
            ContentTextView = FindViewById<TextView>(Resource.Id.risk_content);
            DoneSwitch = FindViewById<Switch>(Resource.Id.done_switch);
            DoneInfoTextView = FindViewById<TextView>(Resource.Id.done_info);
            UpdateTextViews();

            DoneSwitch.CheckedChange += (sender, e) =>
            {
                Database.Manager.CheckRiskAsViewed(risk.Id,DoneSwitch.Checked ? 1 : 0);
                UpdateTextViews();
            };


            ImageHeader = FindViewById<ImageView>(Resource.Id.ic_image_header);
            HeaderImagesLayout = FindViewById<RelativeLayout>(Resource.Id.image_header);
            ExpandableImagesLayout = FindViewById<LinearLayout>(Resource.Id.image_expandable);


            int i = 1;
            HeaderImagesLayout.Click += delegate
            {
                if (++i % 2 == 0)
                {
                    ExpandableImagesLayout.Visibility = ViewStates.Visible;
                    ImageHeader.SetImageResource(Resource.Drawable.ic_collapse);
                }
                else
                {
                    ExpandableImagesLayout.Visibility = ViewStates.Gone;
                    ImageHeader.SetImageResource(Resource.Drawable.ic_expand);
                }
            };


            VideoHeader = FindViewById<ImageView>(Resource.Id.ic_video_header);
            HeaderVideosLayout = FindViewById<RelativeLayout>(Resource.Id.video_header);
            ExpandableVideosLayout = FindViewById<LinearLayout>(Resource.Id.video_expandable);
            int j = 1;
            HeaderVideosLayout.Click += delegate
            {
                if (++j % 2 == 0)
                {

                    ExpandableVideosLayout.Visibility = ViewStates.Visible;
                    VideoHeader.SetImageResource(Resource.Drawable.ic_collapse);
                }
                else
                {
                    ExpandableVideosLayout.Visibility = ViewStates.Gone;
                    VideoHeader.SetImageResource(Resource.Drawable.ic_expand);
                }
            };



            NameTextView.Text = risk.Name;
            ContentTextView.Text = risk.Content;
            DoneSwitch.Checked = risk.Viewed == 1;
            ExpandableImagesLayout.Visibility = ViewStates.Gone;
            ExpandableVideosLayout.Visibility = ViewStates.Gone;

            GetRiskMediaInfo(risk);


        }


        private void GetRiskMediaInfo(Risk risk)
        {
            //Getting media for this risk
            var media = Database.Manager.GetMedia(risk.Id, risk.Language);

            if (media != null)
            {
                images = media.Where(m => m.TypeMedia == Enums.TypeMedia.Image).ToList();
                videos = media.Where(m => m.TypeMedia == Enums.TypeMedia.Video).ToList();

                FillVideos();
                FillImages();

                HeaderImagesLayout.Visibility = images.Count > 0 ? ViewStates.Visible : ViewStates.Gone;
                HeaderVideosLayout.Visibility = videos.Count > 0 ? ViewStates.Visible : ViewStates.Gone;


            }
        }

        public void FillImages()
        {
            foreach (var obj in images)
            {
                try
                {
                    var view = LayoutInflater.Inflate(Resource.Layout.MediaImageView, null);
                    TextView contentTextView = view.FindViewById<TextView>(Resource.Id.media_image_content);
                    contentTextView.Text = obj.Content;
                    ImageView image = view.FindViewById<ImageView>(Resource.Id.media_image_img);
                    string documentsPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
                    string localPath = System.IO.Path.Combine(documentsPath, obj.Name);

                    Glide
                     .With(this)
                     .Load(localPath)
                     .SkipMemoryCache(true).DiskCacheStrategy(DiskCacheStrategy.All)
                     .Into(image);


                    ExpandableImagesLayout.AddView(view);
                }
                catch (Exception ex) { }
            }
        }

        public void FillVideos()
        {
            int i = 1;
            foreach (var obj in videos)
            {
                try
                {
                    var view = LayoutInflater.Inflate(Resource.Layout.MediaVideoView, null);
                    TextView contentTextView = view.FindViewById<TextView>(Resource.Id.media_video_content);
                    contentTextView.Text = "Video: " + (!string.IsNullOrWhiteSpace(obj.Content) ? obj.Content : $"number {i++}");
                    contentTextView.PaintFlags = PaintFlags.UnderlineText;
                    contentTextView.MovementMethod = LinkMovementMethod.Instance;
                    contentTextView.Click += delegate
                    {
                        var appIntent = new Intent(Intent.ActionView, Android.Net.Uri.Parse("vnd.youtube:" + obj.Name));
                        var webIntent = new Intent(Intent.ActionView, Android.Net.Uri.Parse("http://www.youtube.com/watch?v=" + obj.Name));
                        try
                        {
                            StartActivity(appIntent);
                        }
                        catch (ActivityNotFoundException ex)
                        {
                            StartActivity(webIntent);
                        }
                    };
                    ExpandableVideosLayout.AddView(view);
                }
                catch { }
            }
        }

        private void UpdateTextViews()
        {

            DoneInfoTextView.Text = GetString(DoneSwitch.Checked ? Resource.String.done : Resource.String.not_done);
        }



        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Android.Resource.Id.Home:
                    {
                        Finish();
                        return true;
                    }
                default:
                    return base.OnOptionsItemSelected(item);
            }
        }
    }
}