﻿using System.Collections.Generic;
using System.Linq;
using Java.Lang;
using Android.Graphics;
using static Android.Hardware.Camera;

namespace ServicesDemo3
{
    public partial class HiddenCamera
    {
        partial void ModifyParameters(Parameters oldParameters)
        {
            //oldParameters.Set("contrast", "0");
            SetMinPreviewSize(oldParameters);
            SetMaxPictureSize(oldParameters);
            SetFlashModeOff(oldParameters);
            SetFocusModeAuto(oldParameters);
            SetSceneModeAuto(oldParameters);
            SetWhiteBalanceAuto(oldParameters);
            SetPictureFormatJpeg(oldParameters);
            oldParameters.JpegQuality = 100;
            oldParameters.SetRotation(90);
        }

        private Size FindMaxSize(IList<Size> sizes)
        {
            Size[] orderByDescending = sizes
                                    .OrderByDescending(x => x.Width)
                                    .ToArray();
            return orderByDescending[0];
        }

        private Size FindMinSize(IList<Size> sizes)
        {
            Size[] orderByDescending = sizes
                                    .OrderBy(x => x.Width)
                                    .ToArray();
            return orderByDescending[0];
        }

        private void SetMinPreviewSize(Parameters oldParameters)
        {
            Size minSize = FindMinSize(oldParameters.SupportedPreviewSizes);
            oldParameters.SetPreviewSize(minSize.Width, minSize.Height);
        }

        private void SetMaxPictureSize(Parameters oldParameters)
        {
            Size size = FindMaxSize(oldParameters.SupportedPictureSizes);
            oldParameters.SetPictureSize(size.Width, size.Height);
        }

        private void SetFlashModeOff(Parameters oldParameters)
        {
            IList<string> supportedFlashModes = oldParameters.SupportedFlashModes;

            if (supportedFlashModes != null &&
                supportedFlashModes.Contains(Parameters.FlashModeOff))
            {
                oldParameters.FlashMode = Parameters.FlashModeOff;
            }
        }

        private void SetFocusModeAuto(Parameters oldParameters)
        {
            IList<string> supportedFocusModes = oldParameters.SupportedFocusModes;

            if (supportedFocusModes != null &&
                supportedFocusModes.Contains(Parameters.FocusModeAuto))
            {
                oldParameters.FocusMode = Parameters.FocusModeAuto;
            }
        }

        private void SetSceneModeAuto(Parameters oldParameters)
        {
            IList<string> supportedSceneModes = oldParameters.SupportedSceneModes;

            if (supportedSceneModes != null && 
                supportedSceneModes.Contains(Parameters.SceneModeAuto))
            {
                oldParameters.SceneMode = Parameters.SceneModeAuto;
            }
        }

        private void SetWhiteBalanceAuto(Parameters oldParameters)
        {
            IList<string> supportedWhiteBalance = oldParameters.SupportedWhiteBalance;

            if (supportedWhiteBalance != null &&
                supportedWhiteBalance.Contains(Parameters.WhiteBalanceAuto))
            {
                oldParameters.WhiteBalance = Parameters.WhiteBalanceAuto;
            }
        }

        private void SetPictureFormatJpeg(Parameters oldParameters)
        {
            var jpeg = (Integer)(int)ImageFormat.Jpeg;
            IList<Integer> supportedPictureFormats = oldParameters.SupportedPictureFormats;

            if (supportedPictureFormats != null &&
                supportedPictureFormats.Contains(jpeg))
            {
                oldParameters.PictureFormat = ImageFormat.Jpeg;
            }
        }
    }
}