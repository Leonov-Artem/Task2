using System;
using Android.App;
using Android.Util;
using Android.Content;
using Android.OS;
using System.Threading;
using Java.Util.Concurrent;
using Java.Lang;

namespace Task2
{
	[Service]
	public class ForegroundService : Service
	{
		static readonly string TAG = "ForegroundService";

		public bool IsStarted { get; private set; }

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
				if (IsStarted)
					Log.Info(TAG, "OnStartCommand: The service is already running.");
				else 
				{
					Log.Info(TAG, "OnStartCommand: The service is starting.");
					RegisterForegroundService();
					IsStarted = true;

                    new System.Threading.Thread(() => SomeTask()).Start();
                }
			}
			else if (intent.Action.Equals(Constants.ACTION_STOP_SERVICE))
			{
				Log.Info(TAG, "OnStartCommand: The service is stopping.");
				StopForeground(true);

                // останавливаем сервис, если он был до этого запущен
				StopSelf(); 
				IsStarted = false;
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
            var notificationManager = (NotificationManager)GetSystemService(Context.NotificationService);
            notificationManager.Cancel(Constants.SERVICE_RUNNING_NOTIFICATION_ID);

            IsStarted = false;
			base.OnDestroy();
		}

        private void RegisterForegroundService()
        {
            var notification = new Notification.Builder(this)
                    .SetContentTitle(Resources.GetString(Resource.String.app_name))
                    .SetContentText(Resources.GetString(Resource.String.notification_text))
                    .SetSmallIcon(Resource.Drawable.ic_stat_name)
                    .SetOngoing(true)
                    .Build();

            if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
                CreateNotificationChannel();

			StartForeground(Constants.SERVICE_RUNNING_NOTIFICATION_ID, notification);
		}

        private void CreateNotificationChannel()
        {
            var notificationChannel = new NotificationChannel("ID", "ChanelName", NotificationImportance.Default);
            notificationChannel.LockscreenVisibility = NotificationVisibility.Secret;
            var notificationManager = (NotificationManager)GetSystemService(NotificationService);
            notificationManager.CreateNotificationChannel(notificationChannel);
        }

        private void SomeTask()
        {
            
        }
    }
}
