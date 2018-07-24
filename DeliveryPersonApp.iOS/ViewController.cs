using DeliveriesApp.Model;
using System;

using UIKit;
using Foundation;
using LocalAuthentication;
using System.Threading.Tasks;

namespace DeliveryPersonApp.iOS
{
    public partial class ViewController : UIViewController
    {
        bool hasLoggedin = false;
        string userId = string.Empty;
        public ViewController(IntPtr handle) : base(handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            // Perform any additional setup after loading the view, typically from a nib.

            signinButton.TouchUpInside += SigninButton_TouchUpInside;
        }

        private async void SigninButton_TouchUpInside(object sender, EventArgs e)
        {
            bool success = CheckLogin();

            if (success)
            {
                BiometricsAuth();
            }
            else
            {
                TraditionalLogin();
            }
        }

        private async void TraditionalLogin()
        {
            userId = await DeliveryPerson.Login(emailTextField.Text, passwordTextField.Text);

            if (string.IsNullOrEmpty(userId))
            {

            }
            else
            {
                NSUserDefaults.StandardUserDefaults.SetString(userId, "userId");
                NSUserDefaults.StandardUserDefaults.Synchronize();
                hasLoggedin = true;
                PerformSegue("loginSegue", this);
            }
        }

        private void BiometricsAuth()
        {
            NSError error;
            var context = new LAContext();
            if(context.CanEvaluatePolicy(LAPolicy.DeviceOwnerAuthenticationWithBiometrics, out error))
            {
                InvokeOnMainThread(async () =>
                {
                    var result = await context.EvaluatePolicyAsync(LAPolicy.DeviceOwnerAuthenticationWithBiometrics, "Login");

                    if(result.Item1)
                    {
                        hasLoggedin = true;
                        PerformSegue("loginSegue", this);
                    }
                    else
                    {
                        TraditionalLogin();
                    }
                });
            }
            else
                TraditionalLogin();
        }

        private bool CheckLogin()
        {
            bool hasId = false;

            userId = NSUserDefaults.StandardUserDefaults.StringForKey("userId");

            if (!string.IsNullOrEmpty(userId))
                hasId = true;

            return hasId;
        }

        public override bool ShouldPerformSegue(string segueIdentifier, NSObject sender)
        {
            if(segueIdentifier == "loginSegue")
            {
                return hasLoggedin;
            }

            return base.ShouldPerformSegue(segueIdentifier, sender);
        }

        public override void PrepareForSegue(UIStoryboardSegue segue, NSObject sender)
        {
            if(segue.Identifier == "loginSegue")
            {
                var destinationVC = segue.DestinationViewController as MainTabBarController;
                destinationVC.userId = userId;
            }

            base.PrepareForSegue(segue, sender);
        }

        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
            // Release any cached data, images, etc that aren't in use.
        }
    }
}