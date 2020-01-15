using Android.Hardware.Camera2;

namespace Task2
{
    /// <summary>
    /// Фотографирование по расписанию.
    /// </summary>
    class CameraSchedule
    {
        const int SECONDS_TO_MILLISECONDS = 1000;
        const int MINUTES_TO_MILLISECONDS = 60000;
        const int HOURS_TO_MILLISECONDS = 3600000;
        const int EVERY_MINUTE = 60000;
        const long DELAY = 0;

        HiddenCamera _hiddenCamera;
        UpdateTimeTask _timeTask;
        long _periodInMilliseconds;

        public CameraSchedule(CameraManager cameraManager, int period = 5, Period photographingPeriod = Period.InSeconds)
        {
            _hiddenCamera = new HiddenCamera(cameraManager);
            _timeTask = new UpdateTimeTask(_hiddenCamera);
            _periodInMilliseconds = ConvertToMilliseconds(period, photographingPeriod);
        }

        private long ConvertToMilliseconds(int period, Period photographingPeriod)
        {
            return photographingPeriod switch
            {
                Period.InSeconds => period * SECONDS_TO_MILLISECONDS,
                Period.InMinutes => period * MINUTES_TO_MILLISECONDS,
                Period.InHours => period * HOURS_TO_MILLISECONDS,
                _ => EVERY_MINUTE,
            };
        }

        public void StartTimerPhotography()
        {
            var timer = new Java.Util.Timer();
            timer.Schedule(_timeTask, DELAY, _periodInMilliseconds);
        }

        public void Stop()
            => _timeTask.Stop();
    }
}