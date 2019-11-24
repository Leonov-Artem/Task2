using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Hardware;
using Android.Hardware.Camera2;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace ServicesDemo3
{
    public class CameraInfo
    {
        CameraManager _cameraManager;
        Camera.CameraInfo _info;
        //IWindowManager _windowManager;
        Dictionary<CameraFacing, int> _fromFacingToID;

        public CameraInfo(CameraManager cameraManager)
        {
            _cameraManager = cameraManager;
            //_windowManager = windowManager;
            _info = new Camera.CameraInfo();
            _fromFacingToID = DictionaryCameraFacing();
        }

        public CameraFacing GetCameraFacing(int cameraID)
        {
            Camera.GetCameraInfo(cameraID, _info);
            return _info.Facing;
        }

        public int GetID(CameraFacing cameraFacing)
            => _fromFacingToID[cameraFacing];

        public int NumberOfCameras()
            => _cameraManager.GetCameraIdList().Length;

        public int GetOrientation(int cameraID)
        {
            Camera.GetCameraInfo(cameraID, _info);
            return _info.Orientation;
        }

        //public int CalculateRotationAngle(int cameraId)
        //{
        //    // определяем насколько повернут экран от нормального положения   
        //    int degrees = GetSurfaceOrientation();
        //    int result = 0;

        //    // получаем инфо по камере cameraId
        //    Camera.GetCameraInfo(cameraId, _info);

        //    if (_info.Facing == Camera.CameraInfo.CameraFacingBack)
        //        result = ((360 - degrees) + _info.Orientation) % 360;
        //    else if (_info.Facing == Camera.CameraInfo.CameraFacingFront)
        //    {
        //        result = (360 + _info.Orientation + degrees) % 360;
        //    }

        //    return result;
        //}

        //private int GetSurfaceOrientation()
        //{
        //    SurfaceOrientation rotation = _windowManager.DefaultDisplay.Rotation;
        //    int degrees = 0;

        //    switch (rotation)
        //    {
        //        case SurfaceOrientation.Rotation0:
        //            degrees = 0;
        //            break;
        //        case SurfaceOrientation.Rotation90:
        //            degrees = 90;
        //            break;
        //        case SurfaceOrientation.Rotation180:
        //            degrees = 180;
        //            break;
        //        case SurfaceOrientation.Rotation270:
        //            degrees = 270;
        //            break;
        //    }

        //    return degrees;
        //}

        private Dictionary<CameraFacing, int> DictionaryCameraFacing()
        {
            var keyValuePairs = new Dictionary<CameraFacing, int>();
            string[] cameraIds = _cameraManager.GetCameraIdList();

            foreach (var id in cameraIds)
            {
                int ID = int.Parse(id);
                keyValuePairs[GetCameraFacing(ID)] = ID;
            }

            return keyValuePairs;
        }
    }
}