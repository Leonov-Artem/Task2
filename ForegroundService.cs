using System;
using Android.App;
using Android.Util;
using Android.Content;
using Android.OS;
using System.Threading;

namespace ServicesDemo3
{
	[Service]
	public class ForegroundService : Service
	{
		static readonly string TAG = typeof(ForegroundService).FullName;

		bool isStarted;

		public override void OnCreate()
		{
			base.OnCreate();
			Log.Info(TAG, "OnCreate: the service is initializing.");
        }

		public override StartCommandResult OnStartCommand(Intent intent, StartCommandFlags flags, int startId)
		{
			if (intent.Action.Equals(Constants.ACTION_START_SERVICE))
			{
				if (isStarted)
				{
					Log.Info(TAG, "OnStartCommand: The service is already running.");
				}
				else 
				{
					Log.Info(TAG, "OnStartCommand: The service is starting.");
					RegisterForegroundService();
					isStarted = true;
				}
			}
			else if (intent.Action.Equals(Constants.ACTION_STOP_SERVICE))
			{
				Log.Info(TAG, "OnStartCommand: The service is stopping.");
				StopForeground(true);
				StopSelf();
				isStarted = false;
			}
			else if (intent.Action.Equals(Constants.ACTION_RESTART_TIMER))
			{
				Log.Info(TAG, "OnStartCommand: Restarting the timer.");
			}

            // Это говорит Android не перезапускать службу, если она уничтожена для восстановления ресурсов.
            return StartCommandResult.Sticky;
		}

		public override IBinder OnBind(Intent intent)
		{
			// Return null because this is a pure started service. A hybrid service would return a binder that would
			// allow access to the GetFormattedStamp() method.
			return null;
		}

		public override void OnDestroy()
		{
			Log.Info(TAG, "OnDestroy: The started service is shutting down.");

			// Удаляем notification из строки состояния.
			var notificationManager = (NotificationManager)GetSystemService(NotificationService);
			notificationManager.Cancel(Constants.SERVICE_RUNNING_NOTIFICATION_ID);

			isStarted = false;
			base.OnDestroy();
		}

		void RegisterForegroundService()
		{
			var notification = new Notification.Builder(this)
				.SetContentTitle(Resources.GetString(Resource.String.app_name))
				.SetContentText(Resources.GetString(Resource.String.notification_text))
				.SetSmallIcon(Resource.Drawable.ic_stat_name)
				.SetContentIntent(BuildIntentToShowMainActivity())
				.SetOngoing(true)
				.AddAction(BuildStopServiceAction())
				.Build();


			// Enlist this instance of the service as a foreground service
			StartForeground(Constants.SERVICE_RUNNING_NOTIFICATION_ID, notification);
		}

        /// <summary>
        /// Создает PendingIntent, который будет вызывать MainActivity.
        /// </summary>
        /// <returns>Содержит intent.</returns>
        PendingIntent BuildIntentToShowMainActivity()
		{
			var notificationIntent = new Intent(this, typeof(MainActivity));
			notificationIntent.SetAction(Constants.ACTION_MAIN_ACTIVITY);
			notificationIntent.SetFlags(ActivityFlags.SingleTop | ActivityFlags.ClearTask);
			notificationIntent.PutExtra(Constants.SERVICE_STARTED_KEY, true);

			var pendingIntent = PendingIntent.GetActivity(this, 0, notificationIntent, PendingIntentFlags.UpdateCurrent);
			return pendingIntent;
		}

        /// <summary>
        /// Создает действие Notification.Action, которое позволит пользователю остановить службу через
        /// уведомление в строке состояния
        /// </summary>
        /// <returns>Выполняет отсановку таймера.</returns>
        Notification.Action BuildStopServiceAction()
		{
			var stopServiceIntent = new Intent(this, GetType());
			stopServiceIntent.SetAction(Constants.ACTION_STOP_SERVICE);
			var stopServicePendingIntent = PendingIntent.GetService(this, 0, stopServiceIntent, 0);

			var builder = new Notification.Action.Builder(Android.Resource.Drawable.IcMediaPause,
														  GetText(Resource.String.stop_service),
														  stopServicePendingIntent);
			return builder.Build();
		}
	}
}
