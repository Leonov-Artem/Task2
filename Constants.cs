using System;
namespace ServicesDemo3
{
	public static class Constants
	{
		public const int SERVICE_RUNNING_NOTIFICATION_ID = 10000;
		public const string SERVICE_STARTED_KEY = "has_service_been_started";
		public const string ACTION_START_SERVICE = "START_SERVICE";
		public const string ACTION_STOP_SERVICE = "STOP_SERVICE";
		public const string ACTION_MAIN_ACTIVITY = "MAIN_ACTIVITY";
        public const string CAMERA_IDENTIFIERS = "CAMERA_IDENTIFIERS";
        public const string NOTITFICATION_CHANNEL_ID = "SOME_CHANNEL_ID";
        public const string NOTIFICATION_CHANNEL_NAME = "ForegroundService";
    }
}
