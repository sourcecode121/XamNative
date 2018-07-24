using CoreLocation;
using DeliveriesApp.Model;
using Foundation;
using MapKit;
using System;
using UIKit;

namespace DeliveryPersonApp.iOS
{
    public partial class PickUpViewController : UIViewController
    {
        public Delivery delivery;
        public string userId;
        CLLocationManager locationManager;
        public PickUpViewController (IntPtr handle) : base (handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            pickupBarButtonItem.Clicked += PickupBarButtonItem_Clicked;
            navigateBarButtonItem.Clicked += NavigateBarButtonItem_Clicked;

            PrepareMap();
        }

        private void NavigateBarButtonItem_Clicked(object sender, EventArgs e)
        {
            var coordinates = new CLLocationCoordinate2D(delivery.OriginLatitude, delivery.OriginLatitude);
            var mapItem = new MKMapItem(new MKPlacemark(coordinates));
            mapItem.Name = "Pick up here";
            mapItem.OpenInMaps();
        }

        private void PrepareMap()
        {
            locationManager = new CLLocationManager();
            locationManager.RequestWhenInUseAuthorization();
            pickupMapView.ShowsUserLocation = true;

            var sapn = new MKCoordinateSpan(0.15, 0.15);
            var coordinates = new CLLocationCoordinate2D(delivery.OriginLatitude, delivery.OriginLatitude);

            pickupMapView.Region = new MKCoordinateRegion(coordinates, sapn);

            pickupMapView.AddAnnotation(new MKPointAnnotation()
            {
                Coordinate = coordinates,
                Title = "Pcik up here"
            });
        }

        private async void PickupBarButtonItem_Clicked(object sender, EventArgs e)
        {
            var haptic = new UINotificationFeedbackGenerator();
            haptic.Prepare();

            bool result = await Delivery.MarkAsPickerUp(delivery, userId);

            UIAlertController alert = null;
            if(result)
            {
                haptic.NotificationOccurred(UINotificationFeedbackType.Success);
                alert = UIAlertController.Create("Success", "Delivery set as picked up", UIAlertControllerStyle.Alert);
            }
            else
            {
                haptic.NotificationOccurred(UINotificationFeedbackType.Error);
                alert = UIAlertController.Create("Failure", "Please try again", UIAlertControllerStyle.Alert);
            }

            alert.AddAction(UIAlertAction.Create("Ok", UIAlertActionStyle.Default, null));
            PresentViewController(alert, true, null);
        }
    }
}