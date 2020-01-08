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

namespace Task2
{
    public class CameraInfo
    {
        CameraManager _cameraManager;
        Camera.CameraInfo _info;

        public CameraInfo(CameraManager cameraManager)
        {
            _cameraManager = cameraManager;
            _info = new Camera.CameraInfo();
        }

        /// <summary>
        /// По ID камеры возвращет информацию о том, фронтальная или задняя это камера.
        /// </summary>
        /// <param name="cameraID"></param>
        /// <returns></returns>
        public CameraFacing GetCameraFacing(int cameraID)
        {
            Camera.GetCameraInfo(cameraID, _info);
            return _info.Facing;
        }

        /// <summary>
        /// Количество камер на телефоне.
        /// </summary>
        /// <returns></returns>
        public int NumberOfCameras()
            => _cameraManager.GetCameraIdList().Length;

        /// <summary>
        /// Возвращает массив из ID камер, которые имеются на телефоне.
        /// </summary>
        /// <returns></returns>
        public int[] GetCameraIdArray()
        {
            string[] IDs = _cameraManager.GetCameraIdList();
            var array = new int[IDs.Length];

            for (int i = 0; i < IDs.Length; i++)
                array[i] = int.Parse(IDs[i]);

            return array;
        }
    }
}