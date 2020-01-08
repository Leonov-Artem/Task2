using Java.Util;

namespace Task2
{
    /// <summary>
    /// Данный класс необходим для выполнения некоторой задачи по расписанию.
    /// Объект этого класса используется в методе Timer.Schedule(...)
    /// </summary>
    class UpdateTimeTask : TimerTask
    {
        HiddenCamera _hiddenCamera;

        public UpdateTimeTask(HiddenCamera hiddenCamera)
            => _hiddenCamera = hiddenCamera;

        public override void Run()
            => _hiddenCamera.TakePhoto();

        public void Stop()
        {
            this.Cancel();
            _hiddenCamera.StopPreviewAndFreeCamera();
        }
    }
}