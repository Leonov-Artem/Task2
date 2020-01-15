using Android.Hardware;
using Android.Hardware.Camera2;
using Android.Util;
using Java.IO;
using Java.Lang;

namespace Task2
{
    public partial class HiddenCamera
    {
        const int SURFACE_TEXTURE_NAME = 10;
        const int JPEG_MAX_QUALITY = 100;

        Camera _camera;
        CameraInfo _cameraInfo;
        RingList<int> _ringList;
        CameraFacing _currentCameraFacing;
        Camera.IShutterCallback _shutterCallback;
        Camera.IPictureCallback _rawPictureCallback;

        private bool CameraIsOpen
        {
            get => _camera != null;
        }

        public HiddenCamera(CameraManager cameraManager)
        {
            _cameraInfo = new CameraInfo(cameraManager);
            int[] cameraIDs = _cameraInfo.GetCameraIdArray();
            _ringList = new RingList<int>(cameraIDs);
            _shutterCallback = null;
            _rawPictureCallback = null;
        }

        /// <summary>
        /// Делает фото и создает jpg-файл в папке Pictures с указанием id камеры.
        /// </summary>
        public void TakePhoto()
        {
            int cameraId = NextCameraId();
            SafeCameraOpen(cameraId);

            if (CameraIsOpen)
            {
                _currentCameraFacing = _cameraInfo.GetCameraFacing(cameraId);
                SetCameraParametersAndStartPreview();
                TakePicture(cameraId);
            }
            else
                BackToPreviousCameraId();
        }

        /// <summary>
        /// Освобождение ресурсов.
        /// </summary>
        public void StopPreviewAndFreeCamera()
        {
            if (CameraIsOpen)
            {
                _camera.StopPreview();
                _camera.Release();
                _camera = null;
            }
        }

        private void ReleaseCamera()
        {
            if (CameraIsOpen)
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
        private void SafeCameraOpen(int id)
        {
            try
            {
                ReleaseCamera();
                _camera = Camera.Open(id);
            }
            catch (Exception e)
            {
                Log.Info(Constants.CAMERA_TAG, "Проблемы с открытием камеры");
                e.PrintStackTrace();
            }
        }

        private void SetCameraParametersAndStartPreview()
        {
            try
            {
                SetCameraParameters();
                _camera.SetPreviewTexture(new Android
                                              .Graphics
                                              .SurfaceTexture(SURFACE_TEXTURE_NAME));
            }
            catch (IOException e)
            {
                e.PrintStackTrace();
            }

            _camera.StartPreview();
        }

        private void SetCameraParameters()
        {
            Camera.Parameters parameters = _camera.GetParameters();
            ModifyParameters(parameters);
            _camera.SetParameters(parameters);
        }

        /// <summary>
        /// Должен вызываться только после SetCameraParametersAndStartPreview(),
        /// если камера открыта.
        /// </summary>
        /// <param name="cameraId"></param>
        private void TakePicture(int cameraId)
            => _camera.TakePicture(
                    _shutterCallback, 
                    _rawPictureCallback, 
                    new PictureCallback(cameraId));

        private int NextCameraId()
            => _ringList.Next;

        private int BackToPreviousCameraId()
            => _ringList.Previous;

        partial void ModifyParameters(Camera.Parameters oldParameters);
    }
}