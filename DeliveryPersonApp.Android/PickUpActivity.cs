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
    [Activity(Label = "PickUpActivity")]
    public class PickUpActivity : Activity, IOnMapReadyCallback
    {
        MapFragment mapFragment;
        Button pickUpButton;
        double lat, lng;
        string userId, deliveryId;

        public void OnMapReady(GoogleMap googleMap)
        {
            MarkerOptions marker = new MarkerOptions();
            marker.SetPosition(new LatLng(lat, lng));
            marker.SetTitle("Pick up here");
            googleMap.AddMarker(marker);

            googleMap.MoveCamera(CameraUpdateFactory.NewLatLngZoom(new LatLng(lat, lng), 12));
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            SetContentView(Resource.Layout.PickUp);

            mapFragment = FragmentManager.FindFragmentById<MapFragment>(Resource.Id.pickupMapFragment);
            pickUpButton = FindViewById<Button>(Resource.Id.pickUpButton);

            pickUpButton.Click += PickUpButton_Click;

            lat = Intent.GetDoubleExtra("latitude", 0);
            lng = Intent.GetDoubleExtra("longitude", 0);
            userId = Intent.GetStringExtra("userId");
            deliveryId = Intent.GetStringExtra("deliveryId");

            mapFragment.GetMapAsync(this);
        }

        private async void PickUpButton_Click(object sender, EventArgs e)
        {
            await Delivery.MarkAsPickerUp(deliveryId, userId);
        }
    }
}