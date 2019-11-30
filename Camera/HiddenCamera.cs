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
    public partial class HiddenCamera
    {
        readonly int NUMBER_OF_CAMERAS;
        readonly int CAMERA_FACING_BACK_ID;
        readonly int CAMERA_FACING_FRONT_ID;

        Camera _camera;
        CameraInfo _cameraInfo;
        CameraFacing _currentCameraFacing;

        public HiddenCamera(CameraManager cameraManager)
        {
            _cameraInfo = new CameraInfo(cameraManager);
            NUMBER_OF_CAMERAS = _cameraInfo.NumberOfCameras();
            CAMERA_FACING_BACK_ID = _cameraInfo.GetID(CameraFacing.Back);
            _currentCameraFacing = CameraFacing.Back;

            if (NUMBER_OF_CAMERAS == 2)
                CAMERA_FACING_FRONT_ID = _cameraInfo.GetID(CameraFacing.Front);
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
                        _camera = Camera.Open(CAMERA_FACING_FRONT_ID);
                        _currentCameraFacing = CameraFacing.Front;
                        break;
                    case CameraFacing.Front:
                        _camera = Camera.Open(CAMERA_FACING_BACK_ID);
                        _currentCameraFacing = CameraFacing.Back;
                        break;
                }
            }
            else
                _camera = Camera.Open(CAMERA_FACING_BACK_ID);
        }

        private  void SetParametersAndTakePhoto()
        {
            try
            {
                Camera.Parameters parameters = _camera.GetParameters();
                ModifyParameters(parameters);

                _camera.SetPreviewTexture(new Android.Graphics.SurfaceTexture(10));
                _camera.SetParameters(parameters);
                _camera.StartPreview();
                _camera.TakePicture(null, null, new PictureCallback());
            }
            catch (IOException)
            {
                Stop();
            }
        }

        public void Stop()
        {
            Release();
            _camera = null;
        }

        partial void ModifyParameters(Camera.Parameters oldParameters);
    }
}