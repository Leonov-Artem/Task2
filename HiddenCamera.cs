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
        readonly int NUMBER_OF_CAMERAS;
        readonly int CAMERA_FACING_BACK;
        readonly int CAMERA_FACING_FRONT;

        Camera _camera;
        CameraInfo _cameraInfo;
        CameraFacing _currentCameraFacing;

        public HiddenCamera(CameraManager cameraManager)
        {
            _cameraInfo = new CameraInfo(cameraManager);
            NUMBER_OF_CAMERAS = _cameraInfo.NumberOfCameras();
            CAMERA_FACING_BACK = _cameraInfo.GetID(CameraFacing.Back);
            _currentCameraFacing = CameraFacing.Back;

            if (NUMBER_OF_CAMERAS == 2)
                CAMERA_FACING_FRONT = _cameraInfo.GetID(CameraFacing.Front);
        }

        public void TakePhoto()
        {
            Release();
            SwitchCamera();
            SetParametersAndTakePhoto();
        }

        private void Release()
        {
            if (_camera != null)
            {
                _camera.StopPreview();
                _camera.Release();
            }
        }

        private void SwitchCamera()
        {
            if (NUMBER_OF_CAMERAS == 2)
            {
                switch (_currentCameraFacing)
                {
                    case CameraFacing.Back:
                        _camera = Camera.Open(CAMERA_FACING_FRONT);
                        _currentCameraFacing = CameraFacing.Front;
                        break;
                    case CameraFacing.Front:
                        _camera = Camera.Open(CAMERA_FACING_BACK);
                        _currentCameraFacing = CameraFacing.Back;
                        break;
                }
            }
            else
                _camera = Camera.Open(CAMERA_FACING_BACK);
        }

        private  void SetParametersAndTakePhoto()
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

        public void Stop()
        {
            Release();
            _camera = null;
        }

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
            newParameters.WhiteBalance = Camera.Parameters.WhiteBalanceAuto;
            newParameters.PictureFormat = Android.Graphics.ImageFormat.Jpeg;
            newParameters.JpegQuality = 100;
            newParameters.SetRotation(90);
            //newParameters.AutoExposureLock = false;
            //newParameters.ExposureCompensation = -12;
            //int angle = _cameraInfo.CalculateRotationAngle(CAMERA_ID);

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