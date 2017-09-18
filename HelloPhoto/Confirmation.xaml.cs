using System;
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
		private string _photoPath;

		public Confirmation()
		{
			InitializeComponent();
		}

		protected override void OnNavigatedTo(NavigationEventArgs e)
		{
			base.OnNavigatedTo(e);

			_photoPath = e.Parameter.ToString();
		}

		#region Navigation

		private void HomeButton_Click(object sender, RoutedEventArgs e)
		{
			Frame.Navigate(typeof(MainPage));
		}

		//private void RetakeButton_Click(object sender, RoutedEventArgs e)
		//{
		//	Frame.Navigate(typeof(PhotoBooth));
		//}

		private void Page_Loaded(object sender, RoutedEventArgs e)
		{
			ConfirmationPhoto.Source = new BitmapImage(
				new Uri(_photoPath, UriKind.Absolute));
		}

		#endregion Navigation
	}
}
