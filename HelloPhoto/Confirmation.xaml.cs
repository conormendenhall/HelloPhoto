using System;
using System.Diagnostics;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace HelloPhoto
{
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class Confirmation : Page
	{
		private string _imagePath { get; set; }

		public Confirmation()
		{
			InitializeComponent();

			ConfirmationPhoto.Loaded += ConfirmationPhoto_Loaded;
		}

		private async void ConfirmationPhoto_Loaded(object sender, RoutedEventArgs e)
		{
			//StorageFile file = await StorageFile.GetFileFromPathAsync(_imagePath);

			//using (IRandomAccessStream fileStream = await file.OpenAsync(FileAccessMode.Read))
			//{
			//	BitmapImage image = new BitmapImage();
			//	image.SetSource(fileStream);
			//	ConfirmationPhoto.Source = image;
			//}

			ConfirmationPhoto.Source = new BitmapImage(new Uri(_imagePath));
		}

		protected override void OnNavigatedTo(NavigationEventArgs e)
		{
			base.OnNavigatedTo(e);

			_imagePath = e.Parameter.ToString();
		}

		#region Navigation

		private void HomeButton_Click(object sender, RoutedEventArgs e)
		{
			Frame.Navigate(typeof(MainPage));
		}

		#endregion Navigation
	}
}
