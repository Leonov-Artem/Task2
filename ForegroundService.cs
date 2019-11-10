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
                // если сервис уже запущен
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

                // останавливаем сервис, если он был до этого запущен
				StopSelf(); 
				isStarted = false;
			}

            // Говорим Android не перезапускать службу, если она уничтожена ради восстановления ресурсов
            return StartCommandResult.Sticky;
		}

		public override IBinder OnBind(Intent intent)
		{
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

		private void RegisterForegroundService()
		{
			var notification = new Notification.Builder(this)
				.SetContentTitle(Resources.GetString(Resource.String.app_name))
				.SetContentText(Resources.GetString(Resource.String.notification_text))
				.SetSmallIcon(Resource.Drawable.ic_stat_name)
				.SetContentIntent(BuildIntentToShowMainActivity())
				.SetOngoing(true)
				.Build();

			StartForeground(Constants.SERVICE_RUNNING_NOTIFICATION_ID, notification);
		}

        /// <summary>
        /// Создает PendingIntent, который будет вызывать MainActivity.
        /// При клике по уведомлению вернемся в MainActivity.
        /// </summary>
        PendingIntent BuildIntentToShowMainActivity()
		{
			var notificationIntent = new Intent(this, typeof(MainActivity));
			notificationIntent.SetAction(Constants.ACTION_MAIN_ACTIVITY);
			notificationIntent.SetFlags(ActivityFlags.SingleTop | ActivityFlags.ClearTask);
			notificationIntent.PutExtra(Constants.SERVICE_STARTED_KEY, true);

			var pendingIntent = PendingIntent.GetActivity(this, 0, notificationIntent, PendingIntentFlags.UpdateCurrent);
			return pendingIntent;
		}
    }
}
