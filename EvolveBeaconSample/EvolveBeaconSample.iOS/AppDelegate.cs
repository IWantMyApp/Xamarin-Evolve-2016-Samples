using Foundation;
using UIKit;
using CoreLocation;

namespace EvolveBeaconSample.iOS
{
    [Register("AppDelegate")]
    public class AppDelegate : UIApplicationDelegate
    {
        public override UIWindow Window { get; set; }

        #region Code to send notification

        private static void RegisterNotifications()
        {
			var settings = UIUserNotificationSettings
				.GetSettingsForTypes(UIUserNotificationType.Alert, new NSSet());
            UIApplication.SharedApplication.RegisterUserNotificationSettings(settings);
        }

        private static void SendNotification(string alertBody)
        {
            var notification = new UILocalNotification { AlertBody = alertBody };
            UIApplication.SharedApplication.PresentLocalNotificationNow(notification);
		}

		private static void SendEnterNotification()
		{
			SendNotification("Your coffee has been ordered and is on it's way!");
		}

		private static void SendExitNotification()
		{
			SendNotification("Goodbye, and enjoy your coffee!");
		}

        #endregion

		private static readonly NSUuid _regionUuid = new NSUuid("569A17A9-5530-9E0B-4600-EA198EA3EF80");

        public CLLocationManager LocationManager { get; private set; }

        public override bool FinishedLaunching(UIApplication application, 
			NSDictionary launchOptions)
        {
            RegisterNotifications();

			LocationManager = new CLLocationManager();

			LocationManager.AuthorizationChanged += BeaconAuthorizationChanged;
            LocationManager.RequestAlwaysAuthorization();

            return true;
        }

		private void BeaconAuthorizationChanged(object sender, CLAuthorizationChangedEventArgs args)
		{
			if (args.Status != CLAuthorizationStatus.AuthorizedAlways) return;

			LocationManager.RegionEntered += (s, e) => SendEnterNotification();
			LocationManager.RegionLeft += (s, e) => SendExitNotification();

			var beaconRegion = new CLBeaconRegion(_regionUuid, "Evolve Coffee");
				
			LocationManager.StartMonitoring(beaconRegion);
			LocationManager.StartRangingBeacons(beaconRegion);
		}
    }
}