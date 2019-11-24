using Android.Hardware;
using Java.IO;
using Android.Icu.Text;
using Java.Util;
using Android.Net;
using System;
using Android.Util;

namespace ServicesDemo3
{
    class PictureCallback : Java.Lang.Object, Camera.IPictureCallback
    {
        public void OnPictureTaken(byte[] data, Camera camera)
        {
            File photoFile = GetOutputMediaFile();
            try
            {
                var fos = new FileOutputStream(photoFile);
                fos.Write(data);
                fos.Close();
                Log.Info("MyCamera", "Фото сделано");
            }
            catch (Exception e)
            {
                _ = e.StackTrace;
            }
        }

        private File GetOutputMediaFile()
        {
            string timeStamp = new SimpleDateFormat("yyyyMMdd_HHmmss").Format(new Date());
            string imageFileName = "JPEG_" + timeStamp + "_";
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