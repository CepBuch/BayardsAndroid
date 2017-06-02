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

namespace Bayards_Android.RiskViewModel
{
    class RiskViewHolder : RecyclerView.ViewHolder
    {
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
                    ExpandableVideosLayout.Visibility = ViewStates.Gone;
                    VideoHeader.SetImageResource(Resource.Drawable.ic_expand);
                }
            };
        }



        private void UpdateTextViews()
        {

            DoneInfoTextView.Text = ItemView.Context.GetString(DoneSwitch.Checked ? Resource.String.done : Resource.String.not_done);
        }



    }
}