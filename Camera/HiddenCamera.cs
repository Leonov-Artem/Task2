using Android.Hardware;
using Android.Hardware.Camera2;
using Android.OS;
using Android.Util;
using Android.Views;
using Java.IO;
using Java.Lang;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Task2
{
    public partial class HiddenCamera
    {
        Camera _camera;
        CameraInfo _cameraInfo;
        RingList<int> _ringList;
        CameraFacing _currentCameraFacing;

        public HiddenCamera(CameraManager cameraManager)
        {
            _cameraInfo = new CameraInfo(cameraManager);
            int[] cameraIDs = _cameraInfo.GetCameraIdArray();
            _ringList = new RingList<int>(cameraIDs);
        }

        public void TakePhoto()
        {
            int cameraId = NextCameraId();
            bool isOpen = SafeCameraOpen(cameraId);

            if (isOpen)
            {
                SetCamera();
                _currentCameraFacing = _cameraInfo.GetCameraFacing(cameraId);
                _camera.TakePicture(null, null, new PictureCallback(cameraId));
            }
        }

        public void StopPreviewAndFreeCamera()
        {
            if (_camera != null)
            {
                _camera.StopPreview();
                _camera.Release();
                _camera = null;
            }
        }

        private void ReleaseCamera()
        {
            if (_camera != null)
            {
                _camera.Release();
                _camera = null;
            }
        }

        private bool SafeCameraOpen(int id)
        {
            var isOpen = false;

            try
            {
                ReleaseCamera();
                _camera = Camera.Open(id);
                isOpen = (_camera != null); 
            }
            catch (Exception e)
            {
                Log.Info(Resource.String.app_name.ToString(), "failed to open Camera");
                e.PrintStackTrace();
            }

            return isOpen;
        }

        private void SetCamera()
        {
            if (_camera != null)
            {
                try
                {
                    SetCameraParameters();
                    _camera.SetPreviewTexture(new Android.Graphics.SurfaceTexture(10));
                }
                catch (IOException e)
                {
                    e.PrintStackTrace();
                }

                _camera.StartPreview();
            }
        }

        private void SetCameraParameters()
        {
            Camera.Parameters parameters = _camera.GetParameters();
            ModifyParameters(parameters);
            _camera.SetParameters(parameters);
        }

        private int NextCameraId()
            => _ringList.Next();

        partial void ModifyParameters(Camera.Parameters oldParameters);
    }
}