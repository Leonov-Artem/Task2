using Java.Util;

namespace ServicesDemo3
{
    class UpdateTimeTask : TimerTask
    {
        HiddenCamera _hiddenCamera;

        public UpdateTimeTask(HiddenCamera hiddenCamera)
            => _hiddenCamera = hiddenCamera;

        public override void Run()
            => _hiddenCamera.TakePhoto();
    }
}