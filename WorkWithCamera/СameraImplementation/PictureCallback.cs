using Android.Hardware;
using Java.IO;
using Android.Icu.Text;
using Java.Util;
using System;
using Android.Util;

namespace Task2
{
    /// <summary>
    /// Объект этого класса нужен в методе TakePicture, который вызывается у Camera.
    /// </summary>
    class PictureCallback : Java.Lang.Object, Camera.IPictureCallback
    {
        private int _cameraID;

        public PictureCallback(int cameraID)
        {
            _cameraID = cameraID;
        }

        /// <summary>
        /// Данный метод вызывается после того, как было сделано фото. 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="camera"></param>
        public void OnPictureTaken(byte[] data, Camera camera)
        {
            File photoFile = GetOutputMediaFile();
            try
            {
                var fos = new FileOutputStream(photoFile);
                fos.Write(data);
                fos.Close();
                Log.Info(Constants.CAMERA_TAG, "Фото сделано");
            }
            catch (Exception e)
            {
                _ = e.StackTrace;
            }
        }

        /// <summary>
        /// Создаем jpg-файл в папке Pictures в памяти телефона.
        /// </summary>
        /// <returns></returns>
        private File GetOutputMediaFile()
        {
            string timeStamp = new SimpleDateFormat("yyyyMMdd_HHmmss").Format(new Date());
            string imageFileName = $"cameraID: {_cameraID}. JPEG_" + timeStamp + "_";
            File storageDir = Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryPictures);
            File image = File.CreateTempFile(
                imageFileName,  /* prefix */
                ".jpg",         /* suffix */
                storageDir      /* directory */
            );

            return image;
        }
    }
}