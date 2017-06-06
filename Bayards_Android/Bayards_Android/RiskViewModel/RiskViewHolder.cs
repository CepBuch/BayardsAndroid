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
using Android.Support.V7.Widget;
using Android.Animation;
using Android.Text.Method;
using Android.Graphics;
using Bayards_Android.Model;
using Com.Bumptech.Glide;
using Com.Bumptech.Glide.Load.Engine;

namespace Bayards_Android.RiskViewModel
{
    class RiskViewHolder : RecyclerView.ViewHolder
    {
        public Context context;

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


        //Defining view of each RecycleView item
        public RiskViewHolder(View itemView, Action<int, int> listener)
            : base(itemView)
        {
            NameTextView = itemView.FindViewById<TextView>(Resource.Id.risk_name);
            ContentTextView = itemView.FindViewById<TextView>(Resource.Id.risk_content);
            DoneSwitch = itemView.FindViewById<Switch>(Resource.Id.done_switch);
            DoneInfoTextView = itemView.FindViewById<TextView>(Resource.Id.done_info);
            UpdateTextViews();



            DoneSwitch.CheckedChange += (sender, e) =>
            {
                listener(base.AdapterPosition, DoneSwitch.Checked ? 1 : 0);
                UpdateTextViews();
            };


            ImageHeader = itemView.FindViewById<ImageView>(Resource.Id.ic_image_header);
            HeaderImagesLayout = itemView.FindViewById<RelativeLayout>(Resource.Id.image_header);
            ExpandableImagesLayout = itemView.FindViewById<LinearLayout>(Resource.Id.image_expandable);
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
                    ExpandableImagesLayout.Visibility =  ViewStates.Gone;
                    //ExpandableImagesLayout.RemoveAllViews();
                    ImageHeader.SetImageResource(Resource.Drawable.ic_expand);
                }
            };


            VideoHeader = itemView.FindViewById<ImageView>(Resource.Id.ic_video_header);
            HeaderVideosLayout = itemView.FindViewById<RelativeLayout>(Resource.Id.video_header);
            ExpandableVideosLayout = itemView.FindViewById<LinearLayout>(Resource.Id.video_expandable);
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
                    //ExpandableVideosLayout.RemoveAllViews();
                    ExpandableVideosLayout.Visibility = ViewStates.Gone;
                    VideoHeader.SetImageResource(Resource.Drawable.ic_expand);
                }
            };
        }

        public void FillImages()
        {
            foreach (var obj in images)
            {
                try
                {
                    var view = LayoutInflater.From(context).Inflate(Resource.Layout.MediaImageView, null);
                    TextView contentTextView = view.FindViewById<TextView>(Resource.Id.media_image_content);
                    contentTextView.Text = obj.Content;
                    ImageView image = view.FindViewById<ImageView>(Resource.Id.media_image_img);
                    string documentsPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
                    string localPath = System.IO.Path.Combine(documentsPath, obj.Name);

                    Glide
                     .With(context)
                     .Load(localPath)
                     .SkipMemoryCache(true).DiskCacheStrategy(DiskCacheStrategy.All)
                     .Into(image);


                    ExpandableImagesLayout.AddView(view);
                }
                catch { }
            }
        }

        public void FillVideos()
        {
            int i = 1;
            foreach (var obj in videos)
            {
                try
                {
                    var view = LayoutInflater.From(context).Inflate(Resource.Layout.MediaVideoView, null);
                    TextView contentTextView = view.FindViewById<TextView>(Resource.Id.media_video_content);
                    contentTextView.Text = "Video: " + (!string.IsNullOrWhiteSpace(obj.Content) ? obj.Content : $"{i++}");
                    contentTextView.PaintFlags = PaintFlags.UnderlineText;
                    contentTextView.MovementMethod = LinkMovementMethod.Instance;
                    contentTextView.Click += delegate
                    {
                        var appIntent = new Intent(Intent.ActionView, Android.Net.Uri.Parse("vnd.youtube:" + obj.Name));
                        var webIntent = new Intent(Intent.ActionView, Android.Net.Uri.Parse("http://www.youtube.com/watch?v=" + obj.Name));
                        try
                        {
                            context.StartActivity(appIntent);
                        }
                        catch (ActivityNotFoundException ex)
                        {
                            context.StartActivity(webIntent);
                        }
                    };
                    ExpandableVideosLayout.AddView(view);
                }
                catch { }
            }
        }

        private void UpdateTextViews()
        {
            DoneInfoTextView.Text = ItemView.Context.GetString(DoneSwitch.Checked ? Resource.String.done : Resource.String.not_done);
        }



    }
}