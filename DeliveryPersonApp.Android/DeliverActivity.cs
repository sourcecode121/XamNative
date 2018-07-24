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
using Android.Gms.Maps;
using DeliveriesApp.Model;
using Android.Gms.Maps.Model;

namespace DeliveryPersonApp.Android
{
    [Activity(Label = "DeliverActivity")]
    public class DeliverActivity : Activity, IOnMapReadyCallback
    {
        MapFragment mapFragment;
        Button deliverButton;

        double lat, lng;
        string deliveryId;

        public void OnMapReady(GoogleMap googleMap)
        {
            MarkerOptions marker = new MarkerOptions();
            marker.SetPosition(new LatLng(lat, lng));
            marker.SetTitle("Deliver here");
            googleMap.AddMarker(marker);

            googleMap.MoveCamera(CameraUpdateFactory.NewLatLngZoom(new LatLng(lat, lng), 12));
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            SetContentView(Resource.Layout.Deliver);

            mapFragment = FragmentManager.FindFragmentById<MapFragment>(Resource.Id.deliverMapFragment);
            deliverButton = FindViewById<Button>(Resource.Id.deliverButton);

            deliverButton.Click += DeliverButton_Click;

            lat = Intent.GetDoubleExtra("latitude", 0);
            lng = Intent.GetDoubleExtra("longitude", 0);
            deliveryId = Intent.GetStringExtra("deliveryId");

            mapFragment.GetMapAsync(this);
        }

        private async void DeliverButton_Click(object sender, EventArgs e)
        {
            await Delivery.MarkAsDelivered(deliveryId);
        }
    }
}