using System;
using Android.App;
using Android.Util;
using Android.Content;
using Android.OS;
using System.Threading;
using Java.Util.Concurrent;
using Java.Lang;
using Java.Util;
using Android.Hardware;
using Android.Hardware.Camera2;
using System.Threading.Tasks;
using Android.Support.V4.App;
using Android.Runtime;

namespace Task2
{
	[Service]
	public class ForegroundService : Service
	{
        CameraSchedule _cameraSchedule;

        public override void OnCreate()
        {
            base.OnCreate();

            var cameraManager = (CameraManager)GetSystemService(Context.CameraService);
            _cameraSchedule = new CameraSchedule(cameraManager, 1, Period.InMinutes);
            _cameraSchedule.StartTimerPhotography();
        }

        public override IBinder OnBind(Intent intent)
            => null;

        [return: GeneratedEnum]
        public override StartCommandResult OnStartCommand(Intent intent, [GeneratedEnum] StartCommandFlags flags, int startId)
        {
            if (intent.Action.Equals(Constants.ACTION_START_SERVICE))
            {
                Notification notification;
                if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
                {
                    CreateNotificationChannel();
                    notification = CreateNotificationWithChannelId();
                }
                else
                    notification = CreateNotification();

                StartForeground(Constants.SERVICE_RUNNING_NOTIFICATION_ID, notification);
            }
            //else if (intent.Action.Equals(Constants.ACTION_STOP_SERVICE))

            return StartCommandResult.Sticky;
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            _cameraSchedule.Stop();
        }

        private void CreateNotificationChannel()
        {
            var notificationChannel = new NotificationChannel
                (
                    Constants.NOTITFICATION_CHANNEL_ID,
                    Constants.NOTIFICATION_CHANNEL_NAME,
                    NotificationImportance.Default
                );
            notificationChannel.LockscreenVisibility = NotificationVisibility.Secret;
            var notificationManager = (NotificationManager)GetSystemService(NotificationService);
            notificationManager.CreateNotificationChannel(notificationChannel);
        }

        private Notification CreateNotification()
        {
            var notification = new Notification.Builder(this)
                    .SetContentTitle(Resources.GetString(Resource.String.app_name))
                    .SetContentText(Resources.GetString(Resource.String.notification_text))
                    .SetSmallIcon(Resource.Drawable.ic_stat_name)
                    .SetOngoing(true)
                    .Build();

            return notification;
        }

        private Notification CreateNotificationWithChannelId()
        {
            var notification = new Notification.Builder(this, Constants.NOTITFICATION_CHANNEL_ID)
               .SetContentTitle(Resources.GetString(Resource.String.app_name))
               .SetContentText(Resources.GetString(Resource.String.notification_text))
               .SetSmallIcon(Resource.Drawable.ic_stat_name)
               .SetOngoing(true)
               .Build();

            return notification;
        }
    }
}
