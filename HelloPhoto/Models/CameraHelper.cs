using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Media.Capture;
using Windows.System.Display;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace HelloPhoto.Models
{
    public static class CameraHelper
    {
        private static bool CameraInitialized { get; set; }
        private static bool _externalCamera { get; set; }
        private static bool _isRecording { get; set; }
        private static bool _isPreviewing { get; set; }
        private static bool _mirroringPreview { get; set; }

        private static readonly DisplayRequest _displayRequest = new DisplayRequest();

        private static readonly Guid RotationKey = new Guid("C380465D-2271-428C-9B83-ECEA3B4A85C1");
        private static CameraRotationHelper _rotationHelper { get; set; }
        private static MediaCapture MediaCapture { get; set; }
        private static CaptureElement PreviewControl { get; set; }

        public static bool CameraReady => CameraInitialized && !MediaCapture.MediaCaptureSettings.ConcurrentRecordAndPhotoSupported;

        private static CoreDispatcher Dispatcher { get; set; }

        public static MediaCapture GetInstance(CoreDispatcher dispatcher, CaptureElement previewControl)
        {
            if (!CameraInitialized)
            {
                InitializeCamera();
            }

            Dispatcher = dispatcher;
            PreviewControl = previewControl;

            return MediaCapture;
        }

        private static async void InitializeCamera()
        {

            if (MediaCapture == null)
            {
                // Attempt to get the back camera if one is available, but use any camera device if not
                var cameraDevice = await FindCameraDeviceByPanelAsync(Windows.Devices.Enumeration.Panel.Back);

                if (cameraDevice == null)
                {
                    Debug.WriteLine("No camera device found!");
                    return;
                }

                // Create MediaCapture and its settings
                MediaCapture = new MediaCapture();

                // Register for a notification when video recording has reached the maximum time and when something goes wrong
                MediaCapture.RecordLimitationExceeded += MediaCapture_RecordLimitationExceeded;
                MediaCapture.Failed += MediaCapture_Failed;

                var settings = new MediaCaptureInitializationSettings { VideoDeviceId = cameraDevice.Id };

                // Initialize MediaCapture
                try
                {
                    await MediaCapture.InitializeAsync(settings);
                    CameraInitialized = true;
                }
                catch (UnauthorizedAccessException)
                {
                    Debug.WriteLine("The app was denied access to the camera");
                }

                // If initialization succeeded, start the preview
                if (CameraInitialized)
                {

                    //var res = resolutions.Select(x => ((Windows.Media.MediaProperties.VideoEncodingProperties) x).Height + "x" +
                    //                                  ((Windows.Media.MediaProperties.VideoEncodingProperties) x).Width + " - " +
                    //                                  ((Windows.Media.MediaProperties.VideoEncodingProperties) x).Bitrate + " - " +
                    //                                  ((Windows.Media.MediaProperties.VideoEncodingProperties)x).FrameRate.Numerator + " - " +
                    //                                  ((Windows.Media.MediaProperties.VideoEncodingProperties)x).FrameRate.Denominator);

                    //var nice = string.Join("\r\n",res.ToList());

                    // get available resolutions
                    var resolutions = MediaCapture.VideoDeviceController.GetAvailableMediaStreamProperties(MediaStreamType.Photo).ToList();

                    // set used resolution
#if !DEBUG
                    await MediaCapture.VideoDeviceController.SetMediaStreamPropertiesAsync(MediaStreamType.Photo, resolutions[33]);
#endif
                    // Figure out where the camera is located
                    if (cameraDevice.EnclosureLocation == null || cameraDevice.EnclosureLocation.Panel == Windows.Devices.Enumeration.Panel.Unknown)
                    {
                        // No information on the location of the camera, assume it's an external camera, not integrated on the device
                        _externalCamera = true;
                    }
                    else
                    {
                        // Camera is fixed on the device
                        _externalCamera = false;

                        // Only mirror the preview if the camera is on the front panel
                        _mirroringPreview = true; //(cameraDevice.EnclosureLocation.Panel == Windows.Devices.Enumeration.Panel.Front);
                    }

                    // Initialize rotationHelper
                    _rotationHelper = new CameraRotationHelper(cameraDevice.EnclosureLocation);
                    _rotationHelper.OrientationChanged += RotationHelper_OrientationChanged;

                    await StartPreviewAsync();
                    
                }
            }
        }


        /// <summary>
        /// Starts the preview and adjusts it for for rotation and mirroring after making a request to keep the screen on
        /// </summary>
        /// <returns></returns>
        private static async Task StartPreviewAsync()
        {
            // Prevent the device from sleeping while the preview is running
            _displayRequest.RequestActive();

            // Set the preview source in the UI and mirror it if necessary
            PreviewControl.Source = MediaCapture;
            PreviewControl.FlowDirection = _mirroringPreview ? FlowDirection.RightToLeft : FlowDirection.LeftToRight;

            // Start the preview
            await MediaCapture.StartPreviewAsync();
            _isPreviewing = true;

            // Initialize the preview to the current orientation
            if (_isPreviewing)
            {
                await SetPreviewRotationAsync();
            }
        }


        private static async void MediaCapture_RecordLimitationExceeded(MediaCapture sender)
        {
            // This is a notification that recording has to stop, and the app is expected to finalize the recording

            await StopRecordingAsync();
        }

        private static async void MediaCapture_Failed(MediaCapture sender, MediaCaptureFailedEventArgs errorEventArgs)
        {
            Debug.WriteLine("MediaCapture_Failed: (0x{0:X}) {1}", errorEventArgs.Code, errorEventArgs.Message);

            await CleanupCameraAsync();
        }


        /// <summary>
        /// Stops recording a video
        /// </summary>
        /// <returns></returns>
        private static async Task StopRecordingAsync()
        {
            Debug.WriteLine("Stopping recording...");

            _isRecording = false;
            await MediaCapture.StopRecordAsync();

            Debug.WriteLine("Stopped recording!");
        }

        /// <summary>
        /// Cleans up the camera resources (after stopping any video recording and/or preview if necessary) and unregisters from MediaCapture events
        /// </summary>
        /// <returns></returns>
        private static async Task CleanupCameraAsync()
        {
            Debug.WriteLine("CleanupCameraAsync");

            if (CameraInitialized)
            {
                // If a recording is in progress during cleanup, stop it to save the recording
                if (_isRecording)
                {
                    await StopRecordingAsync();
                }

                if (_isPreviewing)
                {
                    // The call to stop the preview is included here for completeness, but can be
                    // safely removed if a call to MediaCapture.Dispose() is being made later,
                    // as the preview will be automatically stopped at that point
                    await StopPreviewAsync();
                }

                CameraInitialized = false;
            }

            if (MediaCapture != null)
            {
                MediaCapture.RecordLimitationExceeded -= MediaCapture_RecordLimitationExceeded;
                MediaCapture.Failed -= MediaCapture_Failed;
                MediaCapture.Dispose();
                MediaCapture = null;
            }

            if (_rotationHelper != null)
            {
                _rotationHelper.OrientationChanged -= RotationHelper_OrientationChanged;
                _rotationHelper = null;
            }
        }


        /// <summary>
        /// Stops the preview and deactivates a display request, to allow the screen to go into power saving modes
        /// </summary>
        /// <returns></returns>
        private static async Task StopPreviewAsync()
        {
            // Stop the preview
            _isPreviewing = false;
            await MediaCapture.StopPreviewAsync();

            // Use the dispatcher because this method is sometimes called from non-UI threads
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                // Cleanup the UI
                PreviewControl.Source = null;

                // Allow the device screen to sleep now that the preview is stopped
                _displayRequest.RequestRelease();
            });
        }

        /// <summary>
        /// Handles an orientation changed event
        /// </summary>
        private static async void RotationHelper_OrientationChanged(object sender, bool updatePreview)
        {
            if (updatePreview)
            {
                await SetPreviewRotationAsync();
            }
        }

        /// <summary>
        /// Gets the current orientation of the UI in relation to the device (when AutoRotationPreferences cannot be honored) and applies a corrective rotation to the preview
        /// </summary>
        private static async Task SetPreviewRotationAsync()
        {
            // Only need to update the orientation if the camera is mounted on the device
            if (_externalCamera) return;

            // Add rotation metadata to the preview stream to make sure the aspect ratio / dimensions match when rendering and getting preview frames
            var rotation = _rotationHelper.GetCameraPreviewOrientation();
            var props = MediaCapture.VideoDeviceController.GetMediaStreamProperties(MediaStreamType.VideoPreview);
            props.Properties.Add(RotationKey, CameraRotationHelper.ConvertSimpleOrientationToClockwiseDegrees(rotation));
            await MediaCapture.SetEncodingPropertiesAsync(MediaStreamType.VideoPreview, props, null);
        }


        /// <summary>
        /// Attempts to find and return a device mounted on the panel specified, and on failure to find one it will return the first device listed
        /// </summary>
        /// <param name="desiredPanel">The desired panel on which the returned device should be mounted, if available</param>
        /// <returns></returns>
        private static async Task<DeviceInformation> FindCameraDeviceByPanelAsync(Windows.Devices.Enumeration.Panel desiredPanel)
        {
            // Get available devices for capturing pictures
            var allVideoDevices = await DeviceInformation.FindAllAsync(DeviceClass.VideoCapture);

            // Get the desired camera by panel
            DeviceInformation desiredDevice = allVideoDevices.FirstOrDefault(x => x.EnclosureLocation != null && x.EnclosureLocation.Panel == desiredPanel);

            // If there is no device mounted on the desired panel, return the first device found
            return desiredDevice ?? allVideoDevices.FirstOrDefault();
        }
    }
}
