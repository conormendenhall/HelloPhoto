using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Imaging;
using Windows.Media.Capture;
using Windows.Media.MediaProperties;
using Windows.Storage;
using Windows.Storage.FileProperties;
using Windows.Storage.Streams;
using Windows.System.Display;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace HelloPhoto
{
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class PhotoBooth : Page
	{
		// Folder in which the captures will be stored (initialized in SetupUiAsync)
		private StorageFolder _captureFolder = null;
		// Prevent the screen from sleeping while the camera is running
		private readonly DisplayRequest _displayRequest = new DisplayRequest();

		// MediaCapture and its state variables
		private MediaCapture _mediaCapture;
		private bool _isInitialized;

		// Rotation Helper to simplify handling rotation compensation for the camera streams
		private CameraRotationHelper _rotationHelper;

		public PhotoBooth()
		{
			this.InitializeComponent();
		}

		private async void PhotoButton_Click(object sender, RoutedEventArgs e)
		{
			await TakePhotoAsync();
		}

		/// <summary>
		/// Takes a photo to a StorageFile and adds rotation metadata to it
		/// </summary>
		/// <returns></returns>
		private async Task TakePhotoAsync()
		{
			// While taking a photo, keep the video button enabled only if the camera supports simultaneously taking pictures and recording video
			//VideoButton.IsEnabled = _mediaCapture.MediaCaptureSettings.ConcurrentRecordAndPhotoSupported;

			// Make the button invisible if it's disabled, so it's obvious it cannot be interacted with
			//VideoButton.Opacity = VideoButton.IsEnabled ? 1 : 0;

			var stream = new InMemoryRandomAccessStream();

			Debug.WriteLine("Taking photo...");
			await _mediaCapture.CapturePhotoToStreamAsync(ImageEncodingProperties.CreateJpeg(), stream);

			try
			{
				var file = await _captureFolder.CreateFileAsync("SimplePhoto.jpg", CreationCollisionOption.GenerateUniqueName);
				Debug.WriteLine("Photo taken! Saving to " + file.Path);

				var photoOrientation = CameraRotationHelper.ConvertSimpleOrientationToPhotoOrientation(_rotationHelper.GetCameraCaptureOrientation());

				await ReencodeAndSavePhotoAsync(stream, file, photoOrientation);
				Debug.WriteLine("Photo saved!");
			}
			catch (Exception ex)
			{
				// File I/O errors are reported as exceptions
				Debug.WriteLine("Exception when taking a photo: " + ex.ToString());
			}

			// Done taking a photo, so re-enable the button
			//VideoButton.IsEnabled = true;
			//VideoButton.Opacity = 1;
		}

		/// <summary>
		/// Applies the given orientation to a photo stream and saves it as a StorageFile
		/// </summary>
		/// <param name="stream">The photo stream</param>
		/// <param name="file">The StorageFile in which the photo stream will be saved</param>
		/// <param name="photoOrientation">The orientation metadata to apply to the photo</param>
		/// <returns></returns>
		private static async Task ReencodeAndSavePhotoAsync(IRandomAccessStream stream, StorageFile file, PhotoOrientation photoOrientation)
		{
			using (var inputStream = stream)
			{
				var decoder = await BitmapDecoder.CreateAsync(inputStream);

				using (var outputStream = await file.OpenAsync(FileAccessMode.ReadWrite))
				{
					var encoder = await BitmapEncoder.CreateForTranscodingAsync(outputStream, decoder);

					var properties = new BitmapPropertySet { { "System.Photo.Orientation", new BitmapTypedValue(photoOrientation, PropertyType.UInt16) } };

					await encoder.BitmapProperties.SetPropertiesAsync(properties);
					await encoder.FlushAsync();
				}
			}
		}

		#region Navigation

		private void HomeButton_Click(object sender, RoutedEventArgs e)
		{
			this.Frame.Navigate(typeof(MainPage));
		}

		#endregion Navigation
	}
}
