using Android.Hardware;
using Android.Hardware.Camera2;
using Android.Util;
using Android.Views;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Task2
{
    public class HiddenTakingPhotos
    {
        readonly int CAMERA_ID;

        CameraInfo _cameraInfo;
        PictureCallback _pictureCallback;
        Camera _camera;

        public HiddenTakingPhotos(CameraInfo cameraInfo, CameraFacing cameraFacing)
        {
            _cameraInfo = cameraInfo;
            _pictureCallback = new PictureCallback();
            CAMERA_ID = 1; //_cameraInfo.GetID(cameraFacing);
        }

        public void TakePhoto()
        {
            if (_camera == null)
                _camera = GetCamera();

            Camera.Parameters oldParameters = _camera.GetParameters();
            Camera.Parameters _newParameters = GetModifiedParameters(oldParameters);

            _camera.SetPreviewTexture(new Android.Graphics.SurfaceTexture(10));
            _camera.SetParameters(_newParameters);
            _camera.StartPreview();
            _camera.TakePicture(null, null, _pictureCallback);
        }

        public void Stop()
        {
            if (_camera != null)
            {
                _camera.StopPreview();
                _camera.Release();
            }
            _camera = null;
        }

        private Camera GetCamera()
            => Camera.Open(CAMERA_ID);

        private Camera.Parameters GetModifiedParameters(Camera.Parameters oldParameters)
        {
            Camera.Parameters newParameters = oldParameters;
            Camera.Size size = FindMaxSize(newParameters.SupportedPictureSizes);

            newParameters.SetPreviewSize(640, 480);
            newParameters.SetPictureSize(size.Width, size.Height);
            newParameters.Set("contrast", "0");
            newParameters.FlashMode = Camera.Parameters.FlashModeOff;
            newParameters.FocusMode = Camera.Parameters.FocusModeAuto;
            newParameters.SceneMode = Camera.Parameters.SceneModeAuto;
            //newParameters.AutoExposureLock = false;
            newParameters.WhiteBalance = Camera.Parameters.WhiteBalanceAuto;
            //newParameters.ExposureCompensation = -12;
            newParameters.PictureFormat = Android.Graphics.ImageFormat.Jpeg;
            newParameters.JpegQuality = 100;
            int angle = _cameraInfo.CalculateRotationAngle(CAMERA_ID);
            newParameters.SetRotation(angle);

            return newParameters;
        }

        private Camera.Size FindMaxSize(IList<Camera.Size> sizes)
        {
            Camera.Size[] orderByDescending = sizes
                                    .OrderByDescending(x => x.Width)
                                    .ToArray();
            return orderByDescending[0];
        }
    }
}