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
using DeliveriesApp.Model;

namespace DeliveryPersonApp.Android
{
    public class WaitingFragment : global::Android.Support.V4.App.ListFragment
    {
        List<Delivery> deliveries;
        public override async void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
            deliveries = new List<Delivery>();
            deliveries = await Delivery.GetWaiting();
            ListAdapter = new ArrayAdapter(Activity, global::Android.Resource.Layout.SimpleListItem1, deliveries);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // Use this to return your custom view for this Fragment
            // return inflater.Inflate(Resource.Layout.YourFragment, container, false);

            return base.OnCreateView(inflater, container, savedInstanceState);
        }

        public override void OnListItemClick(ListView lValue, View vValue, int position, long id)
        {
            base.OnListItemClick(lValue, vValue, position, id);

            var selectedDelivery = deliveries[position];

            var userId = (Activity as TabsActivity).userId;
            Intent intent = new Intent(Activity, typeof(PickUpActivity));
            intent.PutExtra("latitude", selectedDelivery.OriginLatitude);
            intent.PutExtra("longitude", selectedDelivery.OriginLongitude);
            intent.PutExtra("userId", userId);
            intent.PutExtra("deliveryId", selectedDelivery.Id);
            StartActivity(intent);
        }
    }
}