using Android.Hardware;
using Android.Hardware.Camera2;
using Android.OS;
using Android.Util;
using Android.Views;
using Java.IO;
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
            Release();
            int currentCameraID = SwitchCamera();
            SetParametersAndTakePhoto(currentCameraID);
        }

        private void Release()
        {
            if (_camera != null)
            {
                _camera.StopPreview();
                _camera.Release();
            }
        }

        private int SwitchCamera()
        {
            int cameraId = NextCameraId();
            _camera = Camera.Open(cameraId);
            _currentCameraFacing = _cameraInfo.GetCameraFacing(cameraId);

            return cameraId;
        }

        private  void SetParametersAndTakePhoto(int currentCameraId)
        {
            try
            {
                Camera.Parameters parameters = _camera.GetParameters();
                ModifyParameters(parameters);

                _camera.SetPreviewTexture(new Android.Graphics.SurfaceTexture(10));
                _camera.SetParameters(parameters);
                _camera.StartPreview();
                _camera.TakePicture(null, null, new PictureCallback(currentCameraId));
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

        private int NextCameraId()
            => _ringList.Next();
    }
}