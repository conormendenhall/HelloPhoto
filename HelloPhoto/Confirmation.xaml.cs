using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace HelloPhoto
{
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class Confirmation : Page
	{
		public Confirmation()
		{
			InitializeComponent();
		}

		#region Navigation

		private void HomeButton_Click(object sender, RoutedEventArgs e)
		{
			Frame.Navigate(typeof(MainPage));
		}

		private void RetakeButton_Click(object sender, RoutedEventArgs e)
		{
			Frame.Navigate(typeof(PhotoBooth));
		}

		private void Page_Loaded(object sender, RoutedEventArgs e)
		{
			var img = sender as Image;

			//img.Width = bitmapImage.DecodePixelWidth = 80; //natural px width of image source
			// don't need to set Height, system maintains aspect ratio, and calculates the other
			// dimension, so long as one dimension measurement is provided
			//var bmp = new BitmapImage
			//{
			//	UriSource = new Uri(img.BaseUri.AbsoluteUri)
			//};

			//ConfirmationPhoto.Source = sender;
		}

		#endregion Navigation
	}
}
