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

        /// <summary>
        /// Делает фото и создает jpg-файл в папке Pictures с указанием id камеры.
        /// </summary>
        public void TakePhoto()
        {
            int cameraId = NextCameraId();
            bool isOpen = SafeCameraOpen(cameraId);

            if (isOpen)
            {
                _currentCameraFacing = _cameraInfo.GetCameraFacing(cameraId);
                SetCameraParametersAndStartPreview();
                TakePicture(cameraId);
            }
            else
                BackToPreviousId();
        }

        /// <summary>
        /// Освобождение ресурсов.
        /// </summary>
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

        /// <summary>
        /// Безопасное получение объекта камеры. Возвращает булевый индикатор об успешности операции.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
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
                Log.Info(Constants.CAMERA_TAG, "Проблемы с открытием камеры");
                e.PrintStackTrace();
            }

            return isOpen;
        }

        private void SetCameraParametersAndStartPreview()
        {
            if (_camera != null)
            {
                try
                {
                    SetCameraParameters();
                    _camera.SetPreviewTexture(new Android
                                                  .Graphics
                                                  .SurfaceTexture(Constants.SURFACE_TEXTURE_NAME));
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

        /// <summary>
        /// Должен вызываться только после SetCameraParametersAndStartPreview().
        /// </summary>
        /// <param name="cameraId"></param>
        private void TakePicture(int cameraId)
            => _camera.TakePicture(null, null, new PictureCallback(cameraId));

        private int NextCameraId()
            => _ringList.Next;

        private int BackToPreviousId()
            => _ringList.Previous;

        partial void ModifyParameters(Camera.Parameters oldParameters);
    }
}