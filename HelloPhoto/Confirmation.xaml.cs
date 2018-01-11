using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using HelloPhoto.Models;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace HelloPhoto
{
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class Confirmation : Page
	{
		private string _imagePath { get; set; }
        private DispatcherTimer _timer;
        private int _basetime;

        public Confirmation()
		{
			InitializeComponent();
            BeginTimer();
            ConfirmationPhoto.Loaded += ConfirmationPhoto_Loaded;

		    hashtags.Text = AdminSettings.Event?.TwitterHashTag ?? "#FunWithIOT";
		}
        
        private void BeginTimer()
        {
            _timer = new DispatcherTimer();
            _timer.Interval = new TimeSpan(0, 0, 1);
            _timer.Tick += Timer_Tick;

            _basetime = 9;
            _timer.Start();
        }

        private async void ConfirmationPhoto_Loaded(object sender, RoutedEventArgs e)
		{
            StorageFile file = await StorageFile.GetFileFromPathAsync(_imagePath);

            using (IRandomAccessStream fileStream = await file.OpenAsync(FileAccessMode.Read))
            {
                BitmapImage image = new BitmapImage();
                image.SetSource(fileStream);
                ConfirmationPhoto.Source = image;
            }

		    if (AdminSettings.UseOverlay)
		    {
		        OverlayPhoto.Source = await FromBase64(AdminSettings.Event.ImageOverlayBytes);
		        OverlayPhoto.Visibility = Visibility.Visible;
		    }
		    else
		    {
		        OverlayPhoto.Visibility = Visibility.Collapsed;
		    }
        }
        
	    private async Task<ImageSource> FromBase64(byte[] bytes)
	    {

	        var image = bytes.AsBuffer().AsStream().AsRandomAccessStream();

	        // decode image
	        var decoder = await BitmapDecoder.CreateAsync(image);
	        image.Seek(0);

	        // create bitmap
	        var output = new WriteableBitmap((int)decoder.PixelHeight, (int)decoder.PixelWidth);
	        await output.SetSourceAsync(image);
	        return output;
	    }

        protected override void OnNavigatedTo(NavigationEventArgs e)
		{
			base.OnNavigatedTo(e);

			_imagePath = e.Parameter.ToString();
		}

        private void Timer_Tick(object sender, object e)
        {
            _basetime = _basetime - 1;
            countdown.Text = $"Thanks! Your photo will be tweeted shortly!\r\nGoing home in {_basetime.ToString()}s";
            
            if (_basetime == 0)
            {
                _timer.Stop();
                HomeButton_Click(null, null);
            }
        }

        #region Navigation

        private void HomeButton_Click(object sender, RoutedEventArgs e)
		{
            _timer.Stop();

			Frame.Navigate(typeof(MainPage));
		}

		#endregion Navigation
	}
}
