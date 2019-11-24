using Android.App;
using Android.Content;
using Android.OS;
using Android.Widget;
using Android.Util;
using System;
using System.Collections.Generic;
using Android.Content.PM;
using Android.Hardware;
using Android.Hardware.Camera2;
using Newtonsoft.Json;

namespace ServicesDemo3
{
	[Activity(Label = "@string/app_name", MainLauncher = true, Icon = "@mipmap/icon")]
	public class MainActivity : Activity
	{
        static readonly Type SERVICE_TYPE = typeof(ForegroundService);
        readonly string TAG = SERVICE_TYPE.FullName;

		Button _stopServiceButton;
		Button _startServiceButton;
		static Intent _startServiceIntent;
		Intent _stopServiceIntent;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.Main);

            var permissionsToCheck = new string[]
            {
                Android.Manifest.Permission.Camera,
                Android.Manifest.Permission.WriteExternalStorage,
                Android.Manifest.Permission.ForegroundService
            };
            CallNotGrantedPermissions(permissionsToCheck);

            _startServiceIntent = GetIntent(SERVICE_TYPE, Constants.ACTION_START_SERVICE);
            _stopServiceIntent = GetIntent(SERVICE_TYPE, Constants.ACTION_STOP_SERVICE);

            _startServiceButton = FindViewById<Button>(Resource.Id.start_service_button);
            _stopServiceButton = FindViewById<Button>(Resource.Id.stop_service_button);

            _startServiceButton.Click += StartServiceButton_Click;
            _stopServiceButton.Click += StopServiceButton_Click;
        }

		protected override void OnDestroy()
		{
			base.OnDestroy();
		}

		void StartServiceButton_Click(object sender, System.EventArgs e)
		{
            StartForegroundServiceCompat<ForegroundService>(this);
            Log.Info(TAG, "User requested that the service be started.");
		}

        void StopServiceButton_Click(object sender, System.EventArgs e)
        {
            StopService(_stopServiceIntent);
            Log.Info(TAG, "User requested that the service be stopped.");
        }

        private Intent GetIntent(Type type, string action)
        {
            var intent = new Intent(this, type);
            intent.SetAction(action);
            return intent;
        }

        public static void StartForegroundServiceCompat<T>(Context context, Bundle args = null) where T : Service
        {
            if (args != null)
                _startServiceIntent.PutExtras(args);

            if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
                context.StartForegroundService(_startServiceIntent);
            else
                context.StartService(_startServiceIntent);
        }

        private void CallNotGrantedPermissions(string[] permissionsToCheck)
        {
            if (Build.VERSION.SdkInt >= BuildVersionCodes.M)
            {
                var permissionStillNeeded = GetNotGrantedPermissions(permissionsToCheck);
                if (permissionStillNeeded.Length > 0)
                {
                    RequestPermissions(permissionStillNeeded, 5);
                }
            }
        }

        private string[] GetNotGrantedPermissions(string[] permissionsToCheck)
        {
            var permissionStillNeeded = new List<string>();
            for (int i = 0; i < permissionsToCheck.Length; i++)
            {
                if (Permission.Granted != CheckSelfPermission(permissionsToCheck[i]))
                    permissionStillNeeded.Add(permissionsToCheck[i]);
            }

            return permissionStillNeeded.ToArray();
        }
    }
}

