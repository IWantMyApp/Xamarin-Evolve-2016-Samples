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

		private static NSUuid _regionUuid => 
			new NSUuid("569A17A9-5530-9E0B-4600-EA198EA3EF80");

		internal CLLocationManager LocationManager {get; private set;}

        public override bool FinishedLaunching(UIApplication application, 
			NSDictionary launchOptions)
        {
            RegisterNotifications();

			LocationManager = new CLLocationManager();

			LocationManager.AuthorizationChanged += (s, e) =>
			{
				if (e.Status != CLAuthorizationStatus.AuthorizedAlways) return;

				var region = new CLBeaconRegion(_regionUuid, "Evolve Region");

				LocationManager.RegionEntered += (s1, e1) => SendEnterNotification();
				LocationManager.RegionLeft += (s1, e1) => SendExitNotification();

				LocationManager.StartMonitoring(region);
			};

			LocationManager.RequestAlwaysAuthorization();

            return true;
        }
    }
}