using Android.Hardware;
using Android.Hardware.Camera2;
using Android.OS;
using Android.Util;
using Android.Views;
using Java.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ServicesDemo3
{
    public class HiddenCamera
    {
        static Camera _camera;
        static int CURRENT_CAMERA_ID = 1;

        public static void TakePhoto()
        {
            Release();
            SwitchCamera();
            SetParametersAndTakePhoto();
        }

        private static void Release()
        {
            if (_camera != null)
            {
                _camera.StopPreview();
                _camera.Release();
            }
        }

        private static void SwitchCamera()
        {
            switch (CURRENT_CAMERA_ID)
            {
                case 0:
                    _camera = Camera.Open((int)Camera.CameraInfo.CameraFacingFront);
                    CURRENT_CAMERA_ID = 1;
                    break;
                case 1:
                    _camera = Camera.Open((int)Camera.CameraInfo.CameraFacingBack);
                    CURRENT_CAMERA_ID = 0;
                    break;
            }
        }

        private static void SetParametersAndTakePhoto()
        {
            try
            {
                var pictureCallback = new PictureCallback();

                Camera.Parameters oldParameters = _camera.GetParameters();
                Camera.Parameters _newParameters = GetModifiedParameters(oldParameters);

                _camera.SetPreviewTexture(new Android.Graphics.SurfaceTexture(10));
                _camera.SetParameters(_newParameters);
                _camera.StartPreview();
                _camera.TakePicture(null, null, pictureCallback);
            }
            catch (IOException exception)
            {
                Stop();
            }
        }

        public static void Stop()
        {
            if (_camera != null)
            {
                _camera.Release();
                _camera = null;
            }
        }

        private static Camera.Parameters GetModifiedParameters(Camera.Parameters oldParameters)
        {
            Camera.Parameters newParameters = oldParameters;
            Camera.Size size = FindMaxSize(newParameters.SupportedPictureSizes);

            newParameters.SetPreviewSize(640, 480);
            newParameters.SetPictureSize(size.Width, size.Height);
            newParameters.Set("contrast", "0");
            newParameters.FlashMode = Camera.Parameters.FlashModeOff;
            newParameters.FocusMode = Camera.Parameters.FocusModeAuto;
            newParameters.SceneMode = Camera.Parameters.SceneModeAuto;
            newParameters.WhiteBalance = Camera.Parameters.WhiteBalanceAuto;
            newParameters.PictureFormat = Android.Graphics.ImageFormat.Jpeg;
            newParameters.JpegQuality = 100;
            newParameters.SetRotation(90);
            //newParameters.AutoExposureLock = false;
            //newParameters.ExposureCompensation = -12;
            //int angle = _cameraInfo.CalculateRotationAngle(CAMERA_ID);

            return newParameters;
        }

        private static Camera.Size FindMaxSize(IList<Camera.Size> sizes)
        {
            Camera.Size[] orderByDescending = sizes
                                    .OrderByDescending(x => x.Width)
                                    .ToArray();
            return orderByDescending[0];
        }
    }
}