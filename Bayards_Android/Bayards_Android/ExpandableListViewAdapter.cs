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
using Android.Graphics;

namespace Bayards_Android
{
    public class ExpandableListViewAdapter : BaseExpandableListAdapter
    {
        private Context context;
        private List<string> listGroup;
        private Dictionary<string, List<string>> lstChild;
        
        public ExpandableListViewAdapter(Context context, List<string> listGroup, Dictionary<string,List<string>> lstChild)
        {
            this.context = context;
            this.listGroup = listGroup;
            this.lstChild = lstChild;
        }
        public override int GroupCount
        {
            get
            {
                return listGroup.Count;
            }
        }

        public override bool HasStableIds
        {
            get
            {
                return false;
            }
        }

        public override Java.Lang.Object GetChild(int groupPosition, int childPosition)
        {
            var result = new List<string>();
            lstChild.TryGetValue(listGroup[groupPosition], out result);
            return result[childPosition];
        }

        public override long GetChildId(int groupPosition, int childPosition)
        {
            return childPosition;
        }

        public override int GetChildrenCount(int groupPosition)
        {
            var result = new List<string>();
            lstChild.TryGetValue(listGroup[groupPosition], out result);
            return result.Count;
        }

        public override  View GetChildView(int groupPosition, int childPosition, bool isLastChild, View convertView, ViewGroup parent)
        {
            if(convertView == null)
            {
                LayoutInflater inflater = (LayoutInflater)context.GetSystemService(Context.LayoutInflaterService);
                convertView = inflater.Inflate(Resource.Layout.Media_item_layout, null);
            }

            ImageView imageView  = convertView.FindViewById<ImageView>(Resource.Id.item);
            string content = (string)GetChild(groupPosition, childPosition);
            DownloadImage(content, imageView);
            return convertView;
        }


        private async void DownloadImage(string name, ImageView imageview)
        {
            string documentsPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
            string localPath = System.IO.Path.Combine(documentsPath, name);

            BitmapFactory.Options options = new BitmapFactory.Options();
            options.InJustDecodeBounds = true;
            try
            {
                Bitmap bitmap = await BitmapFactory.DecodeFileAsync(localPath, options);
                //options.InSampleSize = options.OutWidth > options.OutHeight ? options.OutHeight / imageview.Height : options.OutWidth / imageview.Width;
                //options.InJustDecodeBounds = false;

                imageview.SetImageBitmap(bitmap);
            }
            catch (Exception ex)
            {

            }


        }

        public override Java.Lang.Object GetGroup(int groupPosition)
        {
            return listGroup[groupPosition];
        }

        public override long GetGroupId(int groupPosition)
        {
            return groupPosition;
        }

        public override View GetGroupView(int groupPosition, bool isExpanded, View convertView, ViewGroup parent)
        {
            if(convertView == null)
            {
                LayoutInflater inflater = (LayoutInflater)context.GetSystemService(Context.LayoutInflaterService);
                convertView = inflater.Inflate(Resource.Layout.Media_group_item, null);
            }
            string textGroup = (string)GetGroup(groupPosition);

            TextView textViewGroup = convertView.FindViewById<TextView>(Resource.Id.group);
            textViewGroup.Text = textGroup;
            return convertView;
        }
        public override bool IsChildSelectable(int groupPosition, int childPosition)
        {
            return true;
        }
    }
}