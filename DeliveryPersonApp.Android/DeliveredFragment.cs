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
    public class DeliveredFragment : global::Android.Support.V4.App.ListFragment
    {
        List<Delivery> deliveries;
        public override async void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here

            deliveries = new List<Delivery>();
            var userId = (Activity as TabsActivity).userId;
            deliveries = await Delivery.GetDelivered(userId);
            ListAdapter = new ArrayAdapter(Activity, global::Android.Resource.Layout.SimpleListItem1, deliveries);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // Use this to return your custom view for this Fragment
            // return inflater.Inflate(Resource.Layout.YourFragment, container, false);

            return base.OnCreateView(inflater, container, savedInstanceState);
        }
    }
}