using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Imaging;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using HelloPhoto.Models;
using HelloPhoto.Repositories;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace HelloPhoto
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AdminPage : Page
    {
        private bool _pageLoading = true;

        public AdminPage()
        {
            this.InitializeComponent();

            PopulateDropdown();

            if (AdminSettings.Event != null)
            {
                eventDropdown.SelectedItem = AdminSettings.Event;
                for (int x=0; x < eventDropdown.Items.Count; x++)
                {
                    var item = eventDropdown.Items[x] as Event;
                    if (item.EventId == AdminSettings.Event.EventId)
                    {
                        eventDropdown.SelectedIndex = x;
                    }
                }
                
                useOverlay.IsChecked = AdminSettings.UseOverlay;
                useSplash.IsChecked = AdminSettings.UseSplash;
                enableFaceReg.IsChecked = AdminSettings.EnableFaceReg;
                prodEnabled.IsChecked = AdminSettings.ProdEnabled;

                ledBrightness.Value = AdminSettings.LEDBrightness;
            }

            var comPorts = new List<string>();
            for (int x = 0; x < 6; x++)
            {
                comPorts.Add($"COM{x}");
            }

            arduinoComDrp.ItemsSource = comPorts;
            arduinoComDrp.SelectedIndex = AdminSettings.ComPort;

            _pageLoading = false;
        }

        private void PopulateDropdown()
        {
            var events = new EventRepository().Get().Result;

            eventDropdown.ItemsSource = events;
        }

        private void ledBrightness_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            if (_pageLoading)
            {
                return;
            }

            //update arduino
            AdminSettings.LEDBrightness = ledBrightness.Value;
        }

        private async void eventDropdown_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //change theme/event
            var evt = eventDropdown.SelectedItem as Event;

            eventHashtag.Text = "#"+evt.TwitterHashTag;

            OverlayImg.Source = await FromBase64(evt.ImageOverlayBytes);
            OverlayImg.Visibility = Visibility.Visible;

            EventImg.Source = await FromBase64(evt.LandingOverlayBytes);
            EventImg.Visibility = Visibility.Visible;

            AdminSettings.Event = evt;
        }

        private void homeBtn_Click(object sender, RoutedEventArgs e)
        {
            AdminSettings.UseOverlay = useOverlay.IsChecked.Value;
            AdminSettings.UseSplash = useSplash.IsChecked.Value;
            AdminSettings.EnableFaceReg = enableFaceReg.IsChecked.Value;
            AdminSettings.ProdEnabled = prodEnabled.IsChecked.Value;
            Frame.Navigate(typeof(MainPage));
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

        private void enableFaceReg_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void arduinoComDrp_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            AdminSettings.ComPort = arduinoComDrp.SelectedIndex;
        }

        private void ProdEnabled_OnChecked(object sender, RoutedEventArgs e)
        {
        }
    }
}
