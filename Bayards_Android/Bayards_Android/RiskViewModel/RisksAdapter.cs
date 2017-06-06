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
using Bayards_Android.Model;
using Android.Graphics;
using Android.Gms.Tasks;
using Java.Lang;
using Com.Bumptech.Glide;
using Android.Text.Method;
using Com.Bumptech.Glide.Load.Engine;

namespace Bayards_Android.RiskViewModel
{
    class RisksAdapter : RecyclerView.Adapter
    {
        public RisksList _risksList;
        public Context context;


        public event Action<Model.Risk, int> ItemClick;
        public RisksAdapter(Context context, RisksList risksList)
        {
            _risksList = risksList;
            this.context = context;

        }
        public override int ItemCount
        {
            get { return _risksList.NumRisks; }
        }

        public override void OnViewDetachedFromWindow(Java.Lang.Object holder)
        {
            //Glide.Clear(holder)
        }


        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            RecyclerView.ViewHolder rh;
            View itemView = LayoutInflater.From(parent.Context).
                           Inflate(Resource.Layout.RiskView, parent, false);
            rh = new RiskViewHolder(itemView, OnClick);
            return rh;
        }
        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            //Modifying base view with exact data
            if (holder is RiskViewHolder)
            {
                RiskViewHolder rh = holder as RiskViewHolder;
                rh.NameTextView.Text = _risksList[position].Name;
                rh.ContentTextView.Text = _risksList[position].Content;
                rh.DoneSwitch.Checked = _risksList[position].Viewed == 1;
                rh.ExpandableImagesLayout.Visibility = ViewStates.Gone;
                rh.ExpandableVideosLayout.Visibility = ViewStates.Gone;


                //Getting media for this risk
                var media = Database.Manager.GetMedia(_risksList[position].Id, _risksList[position].Language);

                if (media != null)
                {
                    var images = media.Where(m => m.TypeMedia == Enums.TypeMedia.Image).ToList();
                    var videos = media.Where(m => m.TypeMedia == Enums.TypeMedia.Video).ToList();

                    rh.HeaderImagesLayout.Visibility = images.Count > 0 ? ViewStates.Visible : ViewStates.Gone;
                    rh.HeaderVideosLayout.Visibility = videos.Count > 0 ? ViewStates.Visible : ViewStates.Gone;
                    rh.images = images;
                    rh.videos = videos;
                    rh.context = context;
                    rh.FillImages();
                    rh.FillVideos();
                }


            }
        }

        


        void OnClick(int position, int isChecked)
        {
            ItemClick?.Invoke(_risksList[position], isChecked);
        }
    }
}