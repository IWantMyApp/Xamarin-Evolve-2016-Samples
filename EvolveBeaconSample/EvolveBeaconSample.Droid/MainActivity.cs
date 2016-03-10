using Android.App;
using Android.OS;
using Android.Gms.Common.Apis;
using Android.Gms.Nearby.Messages;
using System.Threading.Tasks;
using Android.Gms.Nearby;
using Android.Gms.Common;
using Android.Content;
using Android.Widget;
using System.Text;
using static System.Diagnostics.Debug;

namespace EvolveBeaconSample.Droid
{
    public class EddystoneSubscribeCallback : SubscribeCallback
    {
        public override void OnExpired()
        {
            base.OnExpired();
            System.Diagnostics.Debug.WriteLine("Subscribed expired");
        }
    }

    public class EddystoneMessageListener : MessageListener
    {
        private readonly MainActivity _activity;

        public EddystoneMessageListener(MainActivity activity)
        {
            _activity = activity;
        }

        public override void OnFound(Android.Gms.Nearby.Messages.Message message)
        {   
            var contents = Encoding.UTF8.GetString(message.GetContent());
            _activity.UpdateMessage("Found message:\n\n"+
				$"Namespace - '{message.Namespace}'\n"+
				$"Content - '{contents}'.");
        }

        public override void OnLost(Android.Gms.Nearby.Messages.Message message)
        {
            _activity.UpdateMessage("Lost message:\n\n"+
				$"Namespace - '{message.Namespace}'");
        }
    }

    [Activity(Label = "Evolve Coffee", MainLauncher = true, 
		Icon = "@mipmap/icon")]
    public class MainActivity : Activity, GoogleApiClient.IConnectionCallbacks, 
	IResultCallback
    {
        GoogleApiClient _client;
        EddystoneMessageListener _listener;
        EddystoneSubscribeCallback _callback = new EddystoneSubscribeCallback();

        public void UpdateMessage(string messageText)
        {
            FindViewById<TextView>(Resource.Id.message_text_view).Text = messageText;
        }

        protected override async void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.Main);
         
            _client = new GoogleApiClient.Builder(this)
                .AddConnectionCallbacks(this)
                .AddOnConnectionFailedListener(r => WriteLine("Connection failed"))
                .AddApi(NearbyClass.MessagesApi)
                .Build();
			
            await Task.Run(() => _client.BlockingConnect());

            var status = await NearbyClass.Messages.GetPermissionStatusAsync(_client);

            if (status.IsSuccess)
                Subscribe();
            else
                status.StartResolutionForResult(this, 
					ConnectionResult.ResolutionRequired);
        }

        protected override void OnActivityResult(int requestCode, Result resultCode, 
			Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);

            if (requestCode == ConnectionResult.ResolutionRequired && 
				resultCode == Result.Ok)
                Subscribe();
        }

        void Subscribe()
        {
            var filter = new MessageFilter.Builder()
                .IncludeNamespacedType("evolve-eddystone-sample", "string")
                .Build();
            
            var options = new SubscribeOptions.Builder()
                .SetStrategy(Strategy.BleOnly)
                .SetFilter(filter)
                .SetCallback(_callback).Build();
			
			// subscribe in the background
            NearbyClass.Messages.Subscribe(_client, GetPendingIntent(), options)
				.SetResultCallback(this);

			// subscribe in the foreground
            _listener = new EddystoneMessageListener(this);
            NearbyClass.Messages.Subscribe(_client, _listener, options)
				.SetResultCallback(this);
        }

        private PendingIntent GetPendingIntent()
		{
			return PendingIntent.GetService(Application.Context, 0,
				new Intent(Application.Context, typeof(BackgroundSubscribeService)), 
				PendingIntentFlags.UpdateCurrent);
		}

        public void OnConnected(Bundle connectionHint)
        {
            WriteLine("API connected");
        }

        public void OnConnectionSuspended(int cause)
        {
            WriteLine("API suspended");
        }

        public void OnResult(Java.Lang.Object result)
        {
            
        }
    }
}