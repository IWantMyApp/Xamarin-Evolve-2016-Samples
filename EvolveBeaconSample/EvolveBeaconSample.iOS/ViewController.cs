using System;
using UIKit;
using CoreLocation;
using System.Linq;

namespace EvolveBeaconSample.iOS
{
    public partial class ViewController : UIViewController
	{
		public ViewController(IntPtr handle) : base(handle)	{}

#region Color from distance
		private UIColor ColorFromDistance(double distance)
		{
			if (distance < 0.0d)
				return UIColor.Gray;
			else if (distance < 1.0d)
				return UIColor.Green;
			else if (distance < 5.0d)
				return UIColor.Orange;
			else
				return UIColor.Red;
		}
#endregion

		private CLLocationManager LocationManager => 
			((AppDelegate)UIApplication.SharedApplication.Delegate).LocationManager;

		public override void ViewDidLoad()
        {
            base.ViewDidLoad();

        }
    }
}

