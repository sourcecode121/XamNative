using Android.App;
using Android.Widget;
using Android.OS;
using DeliveriesApp.Model;
using Android.Content;
using Android.Support.V4.Hardware.Fingerprint;
using System;
using Android.Support.V4.Content;
using Android;
using Android.Preferences;

namespace DeliveryPersonApp.Android
{
    [Activity(Label = "DeliveryPersonApp.Android", MainLauncher = true, Name = "DeliveryPersonApp.Android.DeliveryPersonApp.Android.MainActivity", Exported = true)]
    [MetaData ("android.app.shortcuts", Resource ="@xml/shortcuts")]
    public class MainActivity : Activity
    {
        EditText emailEditText, passwordEditText;
        Button signinButton, registerButton;
        FingerprintManagerCompat fingerprintManager;
        ISharedPreferences preferences;
        global::Android.Support.V4.OS.CancellationSignal cancellation;
        string userId;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            fingerprintManager = FingerprintManagerCompat.From(this);
            cancellation = new global::Android.Support.V4.OS.CancellationSignal();
            preferences = Application.Context.GetSharedPreferences("UserInfo", FileCreationMode.Private);
            
            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            emailEditText = FindViewById<EditText>(Resource.Id.emailEditText);
            passwordEditText = FindViewById<EditText>(Resource.Id.passwordEditText);
            signinButton = FindViewById<Button>(Resource.Id.signinButton);
            registerButton = FindViewById<Button>(Resource.Id.registerButton);

            signinButton.Click += SigninButton_Click;
            registerButton.Click += RegisterButton_Click;
            
            if(!string.IsNullOrEmpty(Intent?.Data?.LastPathSegment))
            {
                if(Intent.Data.LastPathSegment == "register")
                {
                    StartActivity(typeof(RegisterActivity));
                }
            }
        }

        private void RegisterButton_Click(object sender, System.EventArgs e)
        {
            StartActivity(typeof(RegisterActivity));
        }

        private async void SigninButton_Click(object sender, System.EventArgs e)
        {
            bool canUseFingerprint = CanUseFingerprint();

            if (canUseFingerprint)
            {
                LogUserIn();
            }
            else
            {
                userId = await DeliveryPerson.Login(emailEditText.Text, passwordEditText.Text);

                if (!string.IsNullOrEmpty(userId))
                {
                    try
                    {
                        var preferencesEditor = preferences.Edit();
                        preferencesEditor.PutString("userId", userId);
                        preferencesEditor.Apply();
                    }
                    catch(Exception ex)
                    {

                    }
                    Intent intent = new Intent(this, typeof(TabsActivity));
                    intent.PutExtra("userId", userId);
                    StartActivity(intent);
                }
                else
                    Toast.MakeText(this, "Failure", ToastLength.Long).Show();
            }
        }

        private void LogUserIn()
        {
            var cancellation = new global::Android.Support.V4.OS.CancellationSignal();
            FingerprintManagerCompat.AuthenticationCallback authenticationCallback = new AuthenticationCallback(this, userId);
            Toast.MakeText(this, "Place fingerprint on sensor", ToastLength.Long).Show();
            fingerprintManager.Authenticate(null, 0, cancellation, authenticationCallback, null);
        }

        private bool CanUseFingerprint()
        {
            userId = preferences.GetString("userId", string.Empty);

            if (!string.IsNullOrEmpty(userId))
            {
                if (fingerprintManager.IsHardwareDetected)
                {
                    if (fingerprintManager.HasEnrolledFingerprints)
                    {
                        var permissionResult = ContextCompat.CheckSelfPermission(this, Manifest.Permission.UseFingerprint);
                        if (permissionResult == global::Android.Content.PM.Permission.Granted)
                            return true;
                    }
                }
            }

            return false;
        }
    }

    class AuthenticationCallback : FingerprintManagerCompat.AuthenticationCallback
    {
        Activity activity;
        string userId;
        public AuthenticationCallback(Activity activity, string userId)
        {
            this.activity = activity;
        }

        public override void OnAuthenticationSucceeded(FingerprintManagerCompat.AuthenticationResult result)
        {
            base.OnAuthenticationSucceeded(result);

            Intent intent = new Intent(activity, typeof(TabsActivity));
            intent.PutExtra("userId", userId);
            activity.StartActivity(intent);
        }

        public override void OnAuthenticationFailed()
        {
            base.OnAuthenticationFailed();

            Toast.MakeText(activity, "Failure", ToastLength.Long).Show();
        }
    }
}

